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
                CreatedAt = DateTime.UtcNow,
                PerformedBy = createdBy.Id.ToString() // Ensure this is set
            };

            _context.WorkflowInstances.Add(instance);
            await _context.SaveChangesAsync();

            return await LoadInstanceWithRelatedDataAsync(instance.Id);
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
                    .Include(i => i.CurrentStep)
                    .Include(i => i.WorkflowStatus)
                    .Include(i => i.Transitions)
                    .FirstOrDefaultAsync(i => i.Id == instanceId);

                if (instance == null)
                    throw new Exception($"Instance {instanceId} not found.");

                var currentStep = await _context.WorkflowSteps
                    .FirstOrDefaultAsync(s => s.Id == instance.CurrentStepId)
                    ?? throw new Exception("Current step not found.");

                // Set PerformedBy on instance
                instance.PerformedBy = currentUser.Id.ToString();
                instance.UpdatedBy = currentUser.Id.ToString();
                instance.UpdatedAt = DateTime.UtcNow;

                // Set PerformedBy on current step
                currentStep.PerformedBy = currentUser.Id.ToString();
                currentStep.UpdatedBy = currentUser.Id.ToString();
                currentStep.UpdatedAt = DateTime.UtcNow;

                // Create WorkFlowInstanceStep with PerformedByUserId
                var instanceStep = new WorkFlowInstanceStep
                {
                    InstanceId = instance.Id,
                    StepId = currentStep.Id,
                    PerformedByUserId = currentUser.Id.ToString(), // Ensure this is set
                    WorkflowStatusId = instance.WorkflowStatusId,
                    Comments = $"Action performed",
                    CompletedAt = DateTime.UtcNow,
                    CreatedBy = currentUser.Id.ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                _context.WorkFlowInstanceSteps.Add(instanceStep);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (await LoadInstanceWithRelatedDataAsync(instance.Id), null);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        //public async Task<(WorkflowInstance CurrentInstance, WorkflowInstance? NextWorkflowInstance)> MakeActionAsync(
        //    int instanceId, int actionTypeEntityId, ApplicationUser currentUser)
        //{
        //    using var transaction = await _context.Database.BeginTransactionAsync();

        //    try
        //    {
        //        // Load instance and all related data
        //        var instance = await _context.WorkflowInstances
        //            .Include(i => i.Workflow)
        //                .ThenInclude(w => w.Steps)
        //            .Include(i => i.CurrentStep)
        //                .ThenInclude(s => s.workflowStatus)
        //            .Include(i => i.WorkflowStatus)
        //            .Include(i => i.Transitions)
        //            .FirstOrDefaultAsync(i => i.Id == instanceId);

        //        if (instance == null)
        //            throw new Exception($"Instance {instanceId} not found.");

        //        var currentStep = instance.Workflow.Steps
        //            .FirstOrDefault(s => s.Id == instance.CurrentStepId)
        //            ?? throw new Exception("Current step not found.");

        //        // Validate user role
        //        var userRoles = await (
        //            from ur in _context.UserRoles
        //            join r in _context.Roles on ur.RoleId equals r.Id
        //            where ur.UserId == currentUser.Id
        //            select r.Name
        //        ).ToListAsync();

        //        var requiredRole = await _context.AppRoles
        //            .Where(ar => ar.Id == currentStep.AppRoleId)
        //            .Select(ar => ar.Name)
        //            .FirstOrDefaultAsync();

        //        if (requiredRole != null &&
        //            !userRoles.Contains(requiredRole) &&
        //            !userRoles.Contains("Admin"))
        //        {
        //            throw new UnauthorizedAccessException($"Only role '{requiredRole}' can perform this step.");
        //        }

        //        // Load action type
        //        var action = await _context.ActionTypes.FindAsync(actionTypeEntityId)
        //            ?? throw new Exception($"Action type {actionTypeEntityId} not found.");

        //        // Determine next/previous steps
        //        var orderedSteps = instance.Workflow.Steps.OrderBy(s => s.Order).ToList();
        //        var index = orderedSteps.IndexOf(currentStep);

        //        var nextStep = index < orderedSteps.Count - 1 ? orderedSteps[index + 1] : null;
        //        var prevStep = index > 0 ? orderedSteps[index - 1] : null;

        //        // Create transition
        //        var transition = new WorkflowTransition
        //        {
        //            WorkflowId = instance.WorkflowId,
        //            FromStepId = currentStep.Id,
        //            FromStatusId = currentStep.WorkflowStatusId,
        //            ActionTypeEntityId = action.Id,
        //            Timestamp = DateTime.UtcNow,
        //            PerformedBy = currentUser.Id.ToString(),
        //            CreatedBy = currentUser.Id.ToString(),
        //            CreatedAt = DateTime.UtcNow
        //        };

        //        // ---- PROCESS ACTION ----
        //        if (action.Name.Equals("Accept", StringComparison.OrdinalIgnoreCase))
        //        {
        //            if (nextStep != null)
        //            {
        //                instance.CurrentStepId = nextStep.Id;
        //                instance.WorkflowStatusId = nextStep.WorkflowStatusId;

        //                transition.ToStepId = nextStep.Id;
        //                transition.ToStatusId = nextStep.WorkflowStatusId;
        //            }
        //            else
        //            {
        //                // End of workflow
        //                var completed = await _context.WorkflowStatuses
        //                    .FirstOrDefaultAsync(ws => ws.Name == "Completed");

        //                if (completed != null)
        //                    instance.WorkflowStatusId = completed.Id;

        //                transition.ToStatusId = instance.WorkflowStatusId;
        //            }
        //        }
        //        else if (action.Name.Equals("Reject", StringComparison.OrdinalIgnoreCase))
        //        {
        //            if (prevStep != null)
        //            {
        //                instance.CurrentStepId = prevStep.Id;
        //                instance.WorkflowStatusId = prevStep.WorkflowStatusId;

        //                transition.ToStepId = prevStep.Id;
        //                transition.ToStatusId = prevStep.WorkflowStatusId;
        //            }
        //            else
        //            {
        //                var rejected = await _context.WorkflowStatuses
        //                    .FirstOrDefaultAsync(ws => ws.Name == "Rejected");

        //                if (rejected != null)
        //                    instance.WorkflowStatusId = rejected.Id;

        //                transition.ToStatusId = instance.WorkflowStatusId;
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception($"Unsupported action type: {action.Name}");
        //        }

        //        // Audit who performed the step
        //        instance.PerformedBy = currentUser.Id.ToString();
        //        currentStep.PerformedBy = currentUser.Id.ToString();

        //        // Add transition
        //        instance.Transitions.Add(transition);
        //        _context.WorkflowTransitions.Add(transition);

        //        // Log instance step
        //        _context.WorkFlowInstanceSteps.Add(new WorkFlowInstanceStep
        //        {
        //            InstanceId = instance.Id,
        //            StepId = currentStep.Id,
        //            PerformedByUserId = currentUser.Id.ToString(),
        //            WorkflowStatusId = transition.ToStatusId ?? currentStep.WorkflowStatusId,
        //            Comments = $"Action: {action.Name}",
        //            CompletedAt = DateTime.UtcNow,
        //            CreatedBy = currentUser.Id.ToString(),
        //            CreatedAt = DateTime.UtcNow
        //        });

        //        instance.UpdatedBy = currentUser.Id.ToString();
        //        instance.UpdatedAt = DateTime.UtcNow;

        //        await _context.SaveChangesAsync();

        //        // Workflow chaining (optional)
        //        WorkflowInstance? nextWorkflowInstance = null;

        //        if (instance.Workflow.ParentWorkflowId.HasValue)
        //        {
        //            var parentId = instance.Workflow.ParentWorkflowId.Value;

        //            var nextWorkflow = await _context.Workflows
        //                .Where(w => w.ParentWorkflowId == parentId && w.Order > instance.Workflow.Order)
        //                .OrderBy(w => w.Order)
        //                .Include(w => w.Steps)
        //                .FirstOrDefaultAsync();

        //            if (nextWorkflow != null)
        //            {
        //                var firstStep = nextWorkflow.Steps.OrderBy(s => s.Order).First();

        //                nextWorkflowInstance = new WorkflowInstance
        //                {
        //                    WorkflowId = nextWorkflow.Id,
        //                    CurrentStepId = firstStep.Id,
        //                    WorkflowStatusId = firstStep.WorkflowStatusId,
        //                    CreatedAt = DateTime.UtcNow,
        //                    StatusText = $"Pending on {firstStep.Name}"
        //                };

        //                _context.WorkflowInstances.Add(nextWorkflowInstance);
        //                await _context.SaveChangesAsync();
        //            }
        //        }

        //        await transaction.CommitAsync();

        //        return (
        //            await LoadInstanceWithRelatedDataAsync(instance.Id),
        //            nextWorkflowInstance != null ? await LoadInstanceWithRelatedDataAsync(nextWorkflowInstance.Id) : null
        //        );
        //    }
        //    catch
        //    {
        //        await transaction.RollbackAsync();
        //        throw;
        //    }
        //}

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