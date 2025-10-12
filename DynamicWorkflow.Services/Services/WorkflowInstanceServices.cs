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

        // 🟢 Create a new instance (e.g. new PR)
        public async Task<WorkflowInstance> CreateInstanceAsync(int workflowId, ApplicationUser createdBy)
        {
            var workflow = await _context.Workflows
                .Include(w => w.Steps)
                .FirstOrDefaultAsync(w => w.Id == workflowId);

            if (workflow == null)
                throw new Exception($"Workflow with ID {workflowId} not found.");

            var firstStep = workflow.Steps.OrderBy(s => s.Order).FirstOrDefault();
            if (firstStep == null)
                throw new Exception("Workflow has no defined steps.");

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

        // 🟡 Perform an action on current step
        public async Task MakeActionAsync(int instanceId, ActionType action, ApplicationUser currentUser)
        {
            var instance = await _context.WorkflowInstances
                .Include(i => i.Workflow)
                    .ThenInclude(w => w.Steps)
                .Include(i => i.CurrentStep)
                .Include(i => i.Transitions)
                .FirstOrDefaultAsync(i => i.Id == instanceId);

            if (instance == null)
                throw new Exception($"Instance with ID {instanceId} not found.");

            var currentStep = instance.CurrentStep ??
                instance.Workflow?.Steps.FirstOrDefault(s => s.Id == instance.CurrentStepId);

            if (currentStep == null)
                throw new Exception("Current step not found in workflow instance.");

            // 🔒 Role validation
            var userRoles = await (from ur in _context.UserRoles
                                   join r in _context.Roles on ur.RoleId equals r.Id
                                   where ur.UserId == currentUser.Id
                                   select r.Name).ToListAsync();

            var requiredRole = currentStep.AssignedRole.ToString();
            if (!userRoles.Contains(requiredRole) && !userRoles.Contains("Admin"))
                throw new UnauthorizedAccessException($"Only role '{requiredRole}' can perform this step.");

            // 📝 Create a transition
            var transition = new WorkflowTransition
            {
                WorkflowId = instance.WorkflowId,
                FromStepId = currentStep.Id,
                Action = action,
                FromState = currentStep.stepStatus,
                Timestamp = DateTime.UtcNow,
                PerformedBy = $"{currentUser.DisplayName} ({string.Join(',', userRoles)})"
            };

            // 🧭 Determine next step
            var nextStep = instance.Workflow.Steps
                .OrderBy(s => s.Order)
                .FirstOrDefault(s => s.Order > currentStep.Order);

            if (nextStep != null)
            {
                transition.ToStepId = nextStep.Id;
                transition.ToState = Status.Pending;
                instance.CurrentStepId = nextStep.Id;
                instance.CurrentStep = nextStep;
                instance.State = Status.InProgress;
            }
            else
            {
                // ✅ Workflow finished
                transition.ToStepId = currentStep.Id;
                transition.ToState = Status.Completed;
                instance.State = Status.Completed;
            }

            instance.Transitions.Add(transition);
            _context.WorkflowTransitions.Add(transition);
            await _context.SaveChangesAsync();
        }

        // 🟣 Get instance details
        public async Task<WorkflowInstance?> GetByIdAsync(int id)
        {
            return await _context.WorkflowInstances
                .Include(i => i.Workflow)
                    .ThenInclude(w => w.Steps)
                .Include(i => i.CurrentStep)
                .Include(i => i.Transitions)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        // 🟤 Get all instances (for admin or reporting)
        public async Task<List<WorkflowInstance>> GetAllAsync()
        {
            return await _context.WorkflowInstances
                .Include(i => i.Workflow)
                .Include(i => i.CurrentStep)
                .ToListAsync();
        }
    }
}
