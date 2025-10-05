using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Entities.Users;
using DynamicWorkflow.Core.Enums;
using DynamicWorkflow.Core.Interfaces;
using DynamicWorkflow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DynamicWorkflow.Services.Services
{
    public class WorkflowInstanceServices : IworkflowInstanceService
    {
        private readonly ApplicationIdentityDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WorkflowInstanceServices(ApplicationIdentityDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<WorkflowInstance> CreateInstanceAsync(int workflowId, Guid userId)
        {
            var workflow = await _context.Workflows
                .Include(w => w.Steps)
                .FirstOrDefaultAsync(wf => wf.Id == workflowId);

            if (workflow == null)
                throw new Exception("Workflow not found");

            // use Step.Order (logical order) instead of Id
            var firstStep = workflow.Steps.OrderBy(stp => stp.Order).FirstOrDefault();
            if (firstStep == null)
                throw new Exception("Workflow has no steps");

            var instance = new WorkflowInstance
            {
                WorkflowId = workflowId,
                CurrentStepId = firstStep.Id,
                State = Status.Pending,
                CreatedAt = DateTime.UtcNow,
            };

            _context.WorkflowInstances.Add(instance);
            await _context.SaveChangesAsync();

            var instanceStep = new WorkFlowInstanceStep
            {
                InstanceId = instance.Id,
                StepId = firstStep.Id,
                PerformedByUserId = userId.ToString(),
                Status = Status.Pending.ToString(),
            };

            _context.WorkFlowInstanceSteps.Add(instanceStep);
            await _context.SaveChangesAsync();
            return instance;
        }

        // Get Instance By Id
        public async Task<WorkflowInstance?> GetInstanceByIdAsync(int instanceId)
        {
            return await _context.WorkflowInstances
                .Include(i => i.CurrentStep)
                .Include(i => i.Workflow)
                .ThenInclude(w => w.Steps)
                .FirstOrDefaultAsync(i => i.Id == instanceId);

        }

        // Return instances currently assigned to the user (by assigned user or by role)
        public async Task<IList<WorkflowInstance>> GetInstancesForUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return new List<WorkflowInstance>();

            var userRoles = (await _userManager.GetRolesAsync(user)).ToList();

            // include current step and its roles
            var instances = await _context.WorkflowInstances
                .Include(i => i.CurrentStep)
                    .ThenInclude(s => s.Roles)
                .Include(i => i.Workflow)
                .Where(i =>
                    // assigned specifically to this user (if AssignedUserId is Guid? on WorkflowStep)
                    (i.CurrentStep != null && i.CurrentStep.AssignedUserId.HasValue && i.CurrentStep.AssignedUserId.Value == userId)
                    ||
                    // or step's enum AssignedRole matches any of user's roles (by name)
                    (i.CurrentStep != null && i.CurrentStep.AssignedRole != default(Roles) &&
                        userRoles.Any(ur => string.Equals(ur, i.CurrentStep.AssignedRole.ToString(), StringComparison.OrdinalIgnoreCase)))
                    ||
                    // or the step has StepRole entries whose RoleName matches any of user's roles
                    (i.CurrentStep != null && i.CurrentStep.Roles.Any() &&
                        i.CurrentStep.Roles.Select(r => r.RoleName).Any(roleName =>
                            userRoles.Any(ur => string.Equals(ur, roleName, StringComparison.OrdinalIgnoreCase))))
                )
                .ToListAsync();

            return instances;
        }

        //  Move to next Step 
        public async Task<WorkflowInstance> MoveToNextStepAsync(
            int instanceId, Guid userId, ActionType action, string? comments = null)
        {
            var instance = await _context.WorkflowInstances
                .Include(i => i.CurrentStep)
                    .ThenInclude(s => s.OutgoingTransitions)
                    .Include(i => i.Workflow)
                    .ThenInclude(w => w.Steps)
                .FirstOrDefaultAsync(i => i.Id == instanceId);

            if (instance == null)
                throw new KeyNotFoundException("Instance not found");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new KeyNotFoundException("User not found");
            //check assigned role

            // currentStep 
            var currentStep = instance.CurrentStep ?? instance.Workflow.Steps.FirstOrDefault(s => s.Id == instance.CurrentStepId);
            if (currentStep == null)
                throw new InvalidOperationException("Current step not found");

            // get user roles
            var userRoles = (await _userManager.GetRolesAsync(user)).ToList();

            // --- AUTHORIZATION CHECK ---
            // 1) AssignedUserId (override): if present only that user can act
            if (currentStep.AssignedUserId.HasValue)
            {
                if (currentStep.AssignedUserId.Value != userId)
                {
                    throw new UnauthorizedAccessException(
                        $"You are not authorized to perform action on step {currentStep.Name}. Assigned user: {currentStep.AssignedUserId}");
                }
            }
            else
            {
                // build list of allowed role names for this step
                var allowedRoles = new List<string>();

                // a) single enum AssignedRole (if set)
                if (currentStep.AssignedRole != default(Roles))
                {
                    allowedRoles.Add(currentStep.AssignedRole.ToString());
                }

                // b) many-to-many StepRole entries (assume StepRole.RoleName exists)
                if (currentStep.Roles != null && currentStep.Roles.Any())
                {
                    allowedRoles.AddRange(currentStep.Roles.Select(r => r.RoleName));
                }

                // remove empties and dedupe
                allowedRoles = allowedRoles
                    .Where(r => !string.IsNullOrWhiteSpace(r))
                    .Select(r => r!.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (allowedRoles.Any())
                {
                    var hasRole = allowedRoles.Any(ar =>
                        userRoles.Any(ur => string.Equals(ur, ar, StringComparison.OrdinalIgnoreCase)));

                    if (!hasRole)
                    {
                        throw new UnauthorizedAccessException(
                            $"You are not authorized to perform action on step {currentStep.Name}. Required role(s): {string.Join(", ", allowedRoles)}");
                    }
                }
                // else: no assigned user and no assigned roles -> open to anyone (change policy if you want deny-by-default)
            }

            // find transition for the requested action
            var transition = currentStep.OutgoingTransitions.FirstOrDefault(t => t.Action == action);
            if (transition == null)
                throw new InvalidOperationException($"No transition found for action {action}");

            //Updated instance
            instance.CurrentStepId = transition.ToStepId;
            instance.State = transition.ToState;

            // create and add instance step history (the step we moved to)
            var instanceStep = new WorkFlowInstanceStep
            {
                InstanceId = instance.Id,
                StepId = transition.ToStepId,
                PerformedByUserId = userId.ToString(),
                Status = transition.ToState.ToString(),
                Comments = comments,
                CompletedAt = DateTime.UtcNow
            };
            _context.WorkFlowInstanceSteps.Add(instanceStep);

            //  register Action
            var actionLog = new WorkflowInstanceAction
            {
                WorkflowInstanceId = instance.Id,
                WorkflowStepId = instance.CurrentStepId,
                WorkFlowInstanceStep = instanceStep, // navigation property linking
                PerformedByUserId = userId, // assuming this field is Guid
                ActionType = action,
                Comments = comments,
                PerformedAt = DateTime.UtcNow
            };
            _context.WorkflowInstancesAction.Add(actionLog);

            await _context.SaveChangesAsync();
            return instance;


        }
    }
}