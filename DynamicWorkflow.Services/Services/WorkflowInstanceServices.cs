using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Entities.Users;
using DynamicWorkflow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace DynamicWorkflow.Services.Services
{
    public class WorkflowInstanceService
    {
        private readonly ApplicationIdentityDbContext _context;

        public WorkflowInstanceService(ApplicationIdentityDbContext context)
        {
            _context = context;
        }

        public async Task<WorkflowInstance> CreateInstanceAsync(int workflowId, ApplicationUser createdBy)
        {
            var workflow = await _context.Workflows
                .Include(w => w.Steps)
                    .ThenInclude(s => s.workflowStatus)
                .Include(w => w.Steps)
                    .ThenInclude(s => s.appRole)
                .Include(w => w.WorkflowStatus)
                .FirstOrDefaultAsync(w => w.Id == workflowId);

            if (workflow == null)
                throw new Exception($"Workflow with ID {workflowId} not found.");

            var firstStep = workflow.Steps.OrderBy(s => s.Order).FirstOrDefault()
                ?? throw new Exception("Workflow has no defined steps.");

            var instance = new WorkflowInstance
            {
                WorkflowId = workflow.Id,
                Workflow = workflow,
                CurrentStepId = firstStep.Id,
                CurrentStep = firstStep,
                WorkflowStatusId = firstStep.WorkflowStatusId,
                WorkflowStatus = firstStep.workflowStatus,
                StatusText = $"Pending on {firstStep.Name}",
                CreatedBy = createdBy.Id.ToString(),
                CreatedAt = DateTime.UtcNow
            };

            _context.WorkflowInstances.Add(instance);
            await _context.SaveChangesAsync();

            return await _context.WorkflowInstances
        .Include(i => i.Workflow)
            .ThenInclude(w => w.Steps)
                .ThenInclude(s => s.workflowStatus)
        .Include(i => i.Workflow)
            .ThenInclude(w => w.Steps)
                .ThenInclude(s => s.appRole)
        .Include(i => i.CurrentStep)
            .ThenInclude(s => s.workflowStatus)
        .Include(i => i.CurrentStep)
            .ThenInclude(s => s.appRole)
        .Include(i => i.WorkflowStatus)
        .FirstOrDefaultAsync(i => i.Id == instance.Id);
        }

        public async Task<(WorkflowInstance CurrentInstance, WorkflowInstance? NextWorkflowInstance)> MakeActionAsync(
            int instanceId, int actionTypeEntityId, ApplicationUser currentUser)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var instance = await _context.WorkflowInstances
            .Include(i => i.Workflow)
                .ThenInclude(w => w.Steps)
                    .ThenInclude(s => s.workflowStatus)
            .Include(i => i.Workflow)
                .ThenInclude(w => w.Steps)
                    .ThenInclude(s => s.appRole)
            .Include(i => i.CurrentStep)
                .ThenInclude(s => s.workflowStatus)
            .Include(i => i.CurrentStep)
                .ThenInclude(s => s.appRole)
            .Include(i => i.WorkflowStatus)
            .Include(i => i.Transitions)
            .FirstOrDefaultAsync(i => i.Id == instanceId);

                if (instance == null)
                    throw new Exception($"Instance {instanceId} not found.");

                var currentStep = instance.Workflow.Steps.FirstOrDefault(s => s.Id == instance.CurrentStepId)
                    ?? throw new Exception("Current step not found.");

                var userRoles = await (from ur in _context.UserRoles
                                       join r in _context.Roles on ur.RoleId equals r.Id
                                       where ur.UserId == currentUser.Id
                                       select r.Name).ToListAsync();

                var requiredRole = await _context.AppRoles
                    .Where(ar => ar.Id == currentStep.AppRoleId)
                    .Select(ar => ar.Name)
                    .FirstOrDefaultAsync();

                if (requiredRole != null && !userRoles.Contains(requiredRole) && !userRoles.Contains("Admin"))
                    throw new UnauthorizedAccessException($"Only role '{requiredRole}' can perform this step.");

                var orderedSteps = instance.Workflow.Steps.OrderBy(s => s.Order).ToList();
                var currentIndex = orderedSteps.IndexOf(currentStep);
                var nextStep = currentIndex < orderedSteps.Count - 1 ? orderedSteps[currentIndex + 1] : null;
                var previousStep = currentIndex > 0 ? orderedSteps[currentIndex - 1] : null;

                var actionTypeEntity = await _context.ActionTypes.FindAsync(actionTypeEntityId);
                if (actionTypeEntity == null)
                    throw new Exception($"Action type with ID {actionTypeEntityId} not found.");

                var transition = new WorkflowTransition
                {
                    WorkflowId = instance.WorkflowId,
                    FromStepId = currentStep.Id,
                    ActionTypeEntityId = actionTypeEntityId,
                    FromStatusId = currentStep.WorkflowStatusId,
                    Timestamp = DateTime.UtcNow,
                    PerformedBy = currentUser.Id.ToString(),
                    CreatedBy = currentUser.Id.ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                string direction;
                WorkflowInstance? nextWorkflowInstance = null;

                if (actionTypeEntity.Name.Equals("Accept", StringComparison.OrdinalIgnoreCase))
                {
                    if (nextStep != null)
                    {
                        instance.CurrentStepId = nextStep.Id;
                        instance.CurrentStep = nextStep;
                        instance.WorkflowStatusId = nextStep.WorkflowStatusId;
                        transition.ToStepId = nextStep.Id;
                        transition.ToStatusId = nextStep.WorkflowStatusId;
                        direction = "Forward";
                    }
                    else
                    {
                        var completedStatus = await _context.WorkflowStatuses
                            .FirstOrDefaultAsync(ws => ws.Name == "Completed");
                        if (completedStatus != null)
                        {
                            instance.WorkflowStatusId = completedStatus.Id;
                            transition.ToStatusId = completedStatus.Id;
                        }
                        direction = "Completed";
                    }
                }
                else if (actionTypeEntity.Name.Equals("Reject", StringComparison.OrdinalIgnoreCase))
                {
                    if (previousStep != null)
                    {
                        instance.CurrentStepId = previousStep.Id;
                        instance.CurrentStep = previousStep;
                        instance.WorkflowStatusId = previousStep.WorkflowStatusId;
                        transition.ToStepId = previousStep.Id;
                        transition.ToStatusId = previousStep.WorkflowStatusId;
                        direction = "Rollback";
                    }
                    else
                    {
                        var rejectedStatus = await _context.WorkflowStatuses
                            .FirstOrDefaultAsync(ws => ws.Name == "Rejected");
                        if (rejectedStatus != null)
                        {
                            instance.WorkflowStatusId = rejectedStatus.Id;
                            transition.ToStatusId = rejectedStatus.Id;
                        }
                        direction = "StartRollback";
                    }
                }
                else
                {
                    throw new Exception($"Unsupported action type: {actionTypeEntity.Name}");
                }

                instance.UpdatedBy = currentUser.Id.ToString();
                instance.UpdatedAt = DateTime.UtcNow;

                instance.Transitions.Add(transition);
                _context.WorkflowTransitions.Add(transition);

                var instanceStep = new WorkFlowInstanceStep
                {
                    InstanceId = instance.Id,
                    StepId = currentStep.Id,
                    PerformedByUserId = currentUser.Id.ToString(),
                    WorkflowStatusId = transition.ToStatusId ?? currentStep.WorkflowStatusId,
                    Comments = $"Action: {actionTypeEntity.Name}",
                    CompletedAt = DateTime.UtcNow,
                    CreatedBy = currentUser.Id.ToString(),
                    CreatedAt = DateTime.UtcNow
                };
                _context.WorkFlowInstanceSteps.Add(instanceStep);

                await _context.SaveChangesAsync();

                var reloadedInstance = await LoadInstanceWithRelatedDataAsync(instance.Id);
                if (reloadedInstance == null)
                    throw new Exception("Failed to reload instance after action.");

                WorkflowInstance? reloadedNextInstance = null;
                if (nextWorkflowInstance != null)
                {
                    var parentId = instance.Workflow.ParentWorkflowId;
                    var nextWorkflow = await _context.Workflows
                        .Where(w => w.ParentWorkflowId == parentId && w.Order > instance.Workflow.Order)
                        .OrderBy(w => w.Order)
                        .Include(w => w.Steps)
                        .FirstOrDefaultAsync();

                    if (nextWorkflow != null)
                    {
                        var firstStepOfNext = nextWorkflow.Steps.OrderBy(s => s.Order).FirstOrDefault();
                        if (firstStepOfNext != null)
                        {
                            nextWorkflowInstance = new WorkflowInstance
                            {
                                WorkflowId = nextWorkflow.Id,
                                Workflow = nextWorkflow,
                                CurrentStepId = firstStepOfNext.Id,
                                CurrentStep = firstStepOfNext,
                                WorkflowStatusId = firstStepOfNext.WorkflowStatusId,
                                StatusText = $"Pending on {firstStepOfNext.Name}",
                                CreatedBy = currentUser.Id.ToString(),
                                CreatedAt = DateTime.UtcNow
                            };
                            _context.WorkflowInstances.Add(nextWorkflowInstance);
                            await _context.SaveChangesAsync();
                            direction = "CompletedAndChained";
                        }
                    }
                    else
                    {
                        direction = "AllWorkflowsCompleted";
                    }
                    reloadedNextInstance = await LoadInstanceWithRelatedDataAsync(nextWorkflowInstance.Id);
                }

                await transaction.CommitAsync();
                return (reloadedInstance, reloadedNextInstance);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Helper method to load instance with all related data
        private async Task<WorkflowInstance?> LoadInstanceWithRelatedDataAsync(int instanceId)
        {
            return await _context.WorkflowInstances
                .Include(i => i.Workflow)
                    .ThenInclude(w => w.Steps)
                        .ThenInclude(s => s.workflowStatus)
                .Include(i => i.Workflow)
                    .ThenInclude(w => w.Steps)
                        .ThenInclude(s => s.appRole)
                .Include(i => i.CurrentStep)
                    .ThenInclude(s => s.workflowStatus)
                .Include(i => i.CurrentStep)
                    .ThenInclude(s => s.appRole)
                .Include(i => i.WorkflowStatus)
                .Include(i => i.Transitions)
                .FirstOrDefaultAsync(i => i.Id == instanceId);
        }

        public async Task<WorkflowInstance?> GetByIdAsync(int id)
        {
            return await _context.WorkflowInstances
        .Include(i => i.Workflow)
            .ThenInclude(w => w.Steps)
                .ThenInclude(s => s.workflowStatus)
        .Include(i => i.Workflow)
            .ThenInclude(w => w.Steps)
                .ThenInclude(s => s.appRole)
        .Include(i => i.CurrentStep)
            .ThenInclude(s => s.workflowStatus)
        .Include(i => i.CurrentStep)
            .ThenInclude(s => s.appRole)
        .Include(i => i.WorkflowStatus)
        .Include(i => i.Transitions)
        .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<WorkflowInstance>> GetWorkflowChainInstancesAsync(int? parentWorkflowId)
        {
            return await _context.WorkflowInstances
                .Include(i => i.Workflow)
                    .ThenInclude(w => w.Steps)
                .Include(i => i.CurrentStep)
                .Include(i => i.Transitions)
                .Include(i => i.WorkflowStatus)
                .Where(i => i.Workflow.ParentWorkflowId == parentWorkflowId)
                .OrderBy(i => i.Workflow.Order)
                .ToListAsync();
        }
        
        public async Task<bool> IsWorkflowChainCompletedAsync(int? parentWorkflowId)
        {
            var allWorkflows = await _context.Workflows
                .Where(w => w.ParentWorkflowId == parentWorkflowId)
                .OrderBy(w => w.Order)
                .ToListAsync();

            if (!allWorkflows.Any()) return false;

            var completedStatus = await _context.WorkflowStatuses
                .FirstOrDefaultAsync(ws => ws.Name == "Completed");

            if (completedStatus == null) return false;

            foreach (var workflow in allWorkflows)
            {
                var hasCompletedInstance = await _context.WorkflowInstances
                    .AnyAsync(i => i.WorkflowId == workflow.Id && i.WorkflowStatusId == completedStatus.Id);
                if (!hasCompletedInstance) return false;
            }
            return true;
        }
        
        public async Task<WorkflowInstance?> GetActiveInstanceInChainAsync(int? parentWorkflowId)
        {
            var allWorkflows = await _context.Workflows
                .Where(w => w.ParentWorkflowId == parentWorkflowId)
                .OrderBy(w => w.Order)
                .Select(w => w.Id)
                .ToListAsync();

            var inProgressStatus = await _context.WorkflowStatuses
                .FirstOrDefaultAsync(ws => ws.Name == "InProgress");

            if (inProgressStatus == null) return null;

            return await _context.WorkflowInstances
                .Include(i => i.Workflow)
                    .ThenInclude(w => w.Steps)
                .Include(i => i.CurrentStep)
                .Include(i => i.Transitions)
                .Include(i => i.WorkflowStatus)
                .Where(i => allWorkflows.Contains(i.WorkflowId) && i.WorkflowStatusId == inProgressStatus.Id)
                .OrderBy(i => i.Workflow.Order)
                .FirstOrDefaultAsync();
        }
    }
}