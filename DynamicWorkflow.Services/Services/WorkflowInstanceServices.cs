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

        // 🟢 Create new workflow instance
        public async Task<WorkflowInstance> CreateInstanceAsync(int workflowId, ApplicationUser createdBy)
        {
            var workflow = await _context.Workflows
                .Include(w => w.Steps)
                .FirstOrDefaultAsync(w => w.Id == workflowId);

            if (workflow == null)
                throw new Exception($"Workflow with ID {workflowId} not found.");

            var firstStep = workflow.Steps.OrderBy(s => s.Order).FirstOrDefault()
                ?? throw new Exception("Workflow has no defined steps.");

            firstStep.stepStatus = Status.InProgress;

            var instance = new WorkflowInstance
            {
                WorkflowId = workflow.Id,
                Workflow = workflow,
                CurrentStepId = firstStep.Id,
                CurrentStep = firstStep,
                State = Status.InProgress
            };

            _context.WorkflowInstances.Add(instance);
            await _context.SaveChangesAsync();

            return instance;
        }

        // 🟡 Perform action (Accept / Reject)
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

                // 🔐 Role check
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

                // Create transition record
                var transition = new WorkflowTransition
                {
                    WorkflowId = instance.WorkflowId,
                    FromStepId = currentStep.Id,
                    Action = action,
                    FromState = currentStep.stepStatus,
                    Timestamp = DateTime.UtcNow,
                    PerformedBy = $"{currentUser.DisplayName} ({string.Join(',', userRoles)})"
                };

                string direction;

                // 🟢 ACCEPT action
                if (action == ActionType.Accept)
                {
                    currentStep.stepStatus = Status.Accepted;

                    if (nextStep != null)
                    {
                        nextStep.stepStatus = Status.InProgress;
                        instance.CurrentStepId = nextStep.Id;
                        instance.CurrentStep = nextStep;
                        transition.ToStepId = nextStep.Id;
                        transition.ToState = Status.InProgress;
                        direction = "Forward";
                    }
                    else
                    {
                        instance.State = Status.Completed;
                        transition.ToState = Status.Completed;
                        direction = "Completed";
                    }
                }
                // 🔴 REJECT action
                else
                {
                    currentStep.stepStatus = Status.Rejected;

                    if (previousStep != null)
                    {
                        previousStep.stepStatus = Status.InProgress;
                        instance.CurrentStepId = previousStep.Id;
                        instance.CurrentStep = previousStep;
                        transition.ToStepId = previousStep.Id;
                        transition.ToState = Status.InProgress;
                        direction = "Rollback";
                    }
                    else
                    {
                        instance.State = Status.Rejected;
                        transition.ToState = Status.Rejected;
                        direction = "StartRollback";
                    }
                }

                // 🔄 Sync instance state with the current step
                instance.State = instance.CurrentStep.stepStatus;

                // 🔁 Evaluate full workflow state
                if (instance.Workflow.Steps.All(s => s.stepStatus == Status.Accepted))
                    instance.State = Status.Completed;
                else if (instance.Workflow.Steps.Any(s => s.stepStatus == Status.Rejected))
                    instance.State = Status.Rejected;
                else if (instance.Workflow.Steps.Any(s => s.stepStatus == Status.InProgress))
                    instance.State = Status.InProgress;

                // Save transition
                instance.Transitions.Add(transition);
                _context.WorkflowTransitions.Add(transition);
                await _context.SaveChangesAsync();

                // ✅ If completed, move to next workflow
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
                        nextWorkflowInstance = await CreateInstanceAsync(nextWorkflow.Id, currentUser);
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

        // 🟣 Get instance by ID
        public async Task<WorkflowInstance?> GetByIdAsync(int id)
        {
            return await _context.WorkflowInstances
                .Include(i => i.Workflow).ThenInclude(w => w.Steps)
                .Include(i => i.CurrentStep)
                .Include(i => i.Transitions)
                .FirstOrDefaultAsync(i => i.Id == id);
        }
    }
}
