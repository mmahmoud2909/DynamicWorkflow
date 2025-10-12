using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Entities.Users;
using DynamicWorkflow.Core.Enums;
using DynamicWorkflow.Core.Interfaces;
using DynamicWorkflow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace DynamicWorkflow.Services.Services
{
    public class StepService : IWorkflow
    {
        private readonly ApplicationIdentityDbContext _context;

        public StepService(ApplicationIdentityDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Executes an action on a workflow step — restricted by role.😒
        /// </summary>
        public async Task MakeActionAsync(Workflow workflow, int stepId, ActionType action, ApplicationUser currentUser)
        {
            var step = workflow.Steps.FirstOrDefault(s => s.Id == stepId);
            if (step == null)
                throw new Exception($"Step with ID {stepId} not found.");

            // ✔ Get user roles (proper way using join)
            var userRoles = await (from ur in _context.UserRoles
                                   join r in _context.Roles on ur.RoleId equals r.Id
                                   where ur.UserId == currentUser.Id
                                   select r.Name).ToListAsync();

            var requiredRole = step.AssignedRole.ToString();

            // ✅ Authorization check
            if (!userRoles.Contains(requiredRole) && !userRoles.Contains("Admin"))
                throw new UnauthorizedAccessException($"Only role '{requiredRole}' can perform this step.");

            // ✅ Update step status
            switch (action)
            {
                case ActionType.Accept:
                    step.stepStatus = Status.Accepted;
                    break;
                case ActionType.Reject:
                    step.stepStatus = Status.Rejected;
                    break;
                default:
                    step.stepStatus = Status.InProgress;
                    break;
            }

            // ✅ Create transition log
            var transition = new WorkflowTransition
            {
                WorkflowId = workflow.Id,
                FromStepId = step.Id,
                ToStepId = (int)(workflow.Steps.OrderBy(s => s.Order).FirstOrDefault(s => s.Order > step.Order)?.Id),
                Action = action,
                FromState = step.stepStatus,
                ToState = Status.Pending,
                Timestamp = DateTime.UtcNow,
                PerformedBy = $"{currentUser.DisplayName} ({string.Join(',', userRoles)})"
            };

            _context.WorkflowTransitions.Add(transition);

            // ✅ Move next step to Pending
            var nextStep = workflow.Steps.OrderBy(s => s.Order).FirstOrDefault(s => s.Order > step.Order);
            if (nextStep != null)
            {
                nextStep.stepStatus = Status.Pending;
            }
            else
            {
                workflow.Status = Status.Completed;
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Returns all steps of a workflow.
        /// </summary>
        public async Task<List<WorkflowStep>> GetAllStepsAsync(int workflowId)
        {
            var workflow = await _context.Workflows
                .Include(w => w.Steps)
                .FirstOrDefaultAsync(w => w.Id == workflowId);

            if (workflow == null)
                throw new Exception("Workflow not found.");

            return workflow.Steps.OrderBy(s => s.Order).ToList();
        }

        // Interface placeholders (future implementation)
        public Task<WorkflowInstance?> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task SaveAsync(WorkflowInstance instance) => throw new NotImplementedException();
        public Task AddAsync(WorkflowInstance instance) => throw new NotImplementedException();
        public Task UpdateAsync(WorkflowInstance instance) => throw new NotImplementedException();
    }
}
