using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Entities.Users;
using DynamicWorkflow.Core.Enums;
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
                .FirstOrDefaultAsync(w => w.Id == workflowId);

            if (workflow == null)
                throw new Exception($"Workflow with ID {workflowId} not found.");

            var firstStep = workflow.Steps.OrderBy(s => s.Order).FirstOrDefault()
                ?? throw new Exception("Workflow has no defined steps.");

            // Clone the step to avoid modifying shared entity
            var instance = new WorkflowInstance
            {
                WorkflowId = workflow.Id,
                Workflow = workflow,
                CurrentStepId = firstStep.Id,
                CurrentStep = firstStep,
                State = Status.InProgress,
                WorkflowStatusId = (int)Status.InProgress,
                StatusText = $"Pending on {firstStep.Name}"
            };

            _context.WorkflowInstances.Add(instance);
            await _context.SaveChangesAsync();

            return instance;
        }

        public async Task<(WorkflowInstance currentInstance, WorkflowInstance? nextWorkflowInstance)> MakeActionAsync(
            int instanceId, ActionType action, ApplicationUser currentUser)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var instance = await _context.WorkflowInstances
                    .Include(i => i.Workflow).ThenInclude(w => w.Steps)
                    .Include(i => i.CurrentStep)
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

                var requiredRole = currentStep.AssignedRole.ToString();
                if (!userRoles.Contains(requiredRole) && !userRoles.Contains("Admin"))
                    throw new UnauthorizedAccessException($"Only role '{requiredRole}' can perform this step.");

                var orderedSteps = instance.Workflow.Steps.OrderBy(s => s.Order).ToList();
                var currentIndex = orderedSteps.IndexOf(currentStep);
                var nextStep = currentIndex < orderedSteps.Count - 1 ? orderedSteps[currentIndex + 1] : null;
                var previousStep = currentIndex > 0 ? orderedSteps[currentIndex - 1] : null;

                var transition = new WorkflowTransition
                {
                    WorkflowId = instance.WorkflowId,
                    FromStepId = currentStep.Id,
                    Action = action,
                    ActionTypeEntityId = (int)action,
                    FromState = instance.State,
                    Timestamp = DateTime.UtcNow,
                    PerformedBy = $"{currentUser.DisplayName} ({string.Join(',', userRoles)})"
                };

                string direction;
                WorkflowInstance? nextWorkflowInstance = null;

                if (action == ActionType.Accept)
                {
                    if (nextStep != null)
                    {
                        instance.CurrentStepId = nextStep.Id;
                        instance.CurrentStep = nextStep;
                        instance.State = Status.Pending;
                        instance.StatusText = $"Pending on {nextStep.Name}";

                        transition.ToStepId = nextStep.Id;
                        transition.ToState = Status.Pending;
                        direction = "MovedForwardPending";
                    }
                    else
                    {
                        instance.State = Status.Completed;
                        instance.StatusText = "Completed";
                        transition.ToState = Status.Completed;
                        direction = "Completed";
                    }
                }
                else
                {
                    if (previousStep != null)
                    {
                        instance.CurrentStepId = previousStep.Id;
                        instance.CurrentStep = previousStep;
                        instance.State = Status.Pending;
                        instance.StatusText = $"Pending on {previousStep.Name}";

                        transition.ToStepId = previousStep.Id;
                        transition.ToState = Status.Pending;
                        direction = "RollbackPending";
                    }
                    else
                    {
                        instance.State = Status.Rejected;
                        instance.StatusText = $"Rejected by {currentStep.Name}";
                        transition.ToState = Status.Rejected;
                        direction = "RejectedAtStart";
                    }
                }

                //instance.State = instance.CurrentStep.stepStatus;

                //if (instance.Workflow.Steps.All(s => s.stepStatus == Status.Accepted))
                //    instance.State = Status.Completed;
                //else if (instance.Workflow.Steps.Any(s => s.stepStatus == Status.Rejected))
                //    instance.State = Status.Rejected;
                //else if (instance.Workflow.Steps.Any(s => s.stepStatus == Status.InProgress))
                //    instance.State = Status.InProgress;

                instance.Transitions.Add(transition);
                _context.WorkflowTransitions.Add(transition);

                // Save the current workflow changes first
                await _context.SaveChangesAsync();

                // Check for next workflow AFTER current workflow is completed
                if (instance.State == Status.Completed)
                {
                    var parentId = instance.Workflow.ParentWorkflowId;
                    var nextWorkflow = await _context.Workflows
                        .Where(w => w.ParentWorkflowId == parentId && w.Order > instance.Workflow.Order)
                        .OrderBy(w => w.Order)
                        .Include(w => w.Steps)
                        .FirstOrDefaultAsync();

                    if (nextWorkflow != null)
                    {
                        // Always create new instance for next workflow
                        var firstStepOfNext = nextWorkflow.Steps.OrderBy(s => s.Order).FirstOrDefault();
                        if (firstStepOfNext != null)
                        {
                            nextWorkflowInstance = new WorkflowInstance
                            {
                                WorkflowId = nextWorkflow.Id,
                                Workflow = nextWorkflow,
                                CurrentStepId = firstStepOfNext.Id,
                                CurrentStep = firstStepOfNext,
                                State = Status.InProgress,
                                WorkflowStatusId = (int) Status.InProgress,
                                StatusText = $"Pending on {firstStepOfNext.Name}"
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
                }

                await transaction.CommitAsync();

                instance.Workflow.Description = direction;

                return (instance, nextWorkflowInstance);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<WorkflowInstance?> GetByIdAsync(int id)
        {
            return await _context.WorkflowInstances
                .Include(i => i.Workflow).ThenInclude(w => w.Steps)
                .Include(i => i.CurrentStep)
                .Include(i => i.Transitions)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<WorkflowInstance>> GetWorkflowChainInstancesAsync(int? parentWorkflowId)
        {
            return await _context.WorkflowInstances
                .Include(i => i.Workflow).ThenInclude(w => w.Steps)
                .Include(i => i.CurrentStep)
                .Include(i => i.Transitions)
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

            if (!allWorkflows.Any())
                return false;

            foreach (var workflow in allWorkflows)
            {
                var hasCompletedInstance = await _context.WorkflowInstances
                    .AnyAsync(i => i.WorkflowId == workflow.Id && i.State == Status.Completed);

                if (!hasCompletedInstance)
                    return false;
            }

            return true;
        }

        // Get the current active instance for a workflow chain
        public async Task<WorkflowInstance?> GetActiveInstanceInChainAsync(int? parentWorkflowId)
        {
            var allWorkflows = await _context.Workflows
                .Where(w => w.ParentWorkflowId == parentWorkflowId)
                .OrderBy(w => w.Order)
                .Select(w => w.Id)
                .ToListAsync();

            return await _context.WorkflowInstances
                .Include(i => i.Workflow).ThenInclude(w => w.Steps)
                .Include(i => i.CurrentStep)
                .Include(i => i.Transitions)
                .Where(i => allWorkflows.Contains(i.WorkflowId) && i.State == Status.InProgress)
                .OrderBy(i => i.Workflow.Order)
                .FirstOrDefaultAsync();
        }
    }
}