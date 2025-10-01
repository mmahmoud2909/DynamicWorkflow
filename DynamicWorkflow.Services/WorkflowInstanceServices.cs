using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Entities.Users;
using DynamicWorkflow.Core.Enums;
using DynamicWorkflow.Core.Interfaces;
using DynamicWorkflow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DynamicWorkflow.Services
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

            var firstStep = workflow.Steps.OrderBy(stp => stp.Id).FirstOrDefault();
            if (firstStep == null)
                throw new Exception("Workflow has no steps");

            var instance = new WorkflowInstance
            {
                WorkflowId = workflowId,
                CurrentStepId = firstStep.Id,
                State = Status.Pending,
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

        public async Task<WorkflowInstance?> GetInstanceByIdAsync(int instanceId)
        {
            return await _context.WorkflowInstances
                .Include(i => i.CurrentStep)
                .FirstOrDefaultAsync(i => i.Id == instanceId);
        }

        public async Task<IList<WorkflowInstance>> GetInstancesForUserAsync(Guid userId)
        {
            return await _context.WorkflowInstances
                .Include(i => i.CurrentStep)
                .Where(i => i.Transitions.Any(t => t.PerformedBy == userId.ToString()))
                .ToListAsync();
        }

        public async Task<WorkflowInstance> MoveToNextStepAsync(int instanceId, Guid userId, ActionType action, string? comments = null)
        {
            var instance = await _context.WorkflowInstances
                .Include(i => i.CurrentStep)
                    .ThenInclude(s => s.OutgoingTransitions)
                .FirstOrDefaultAsync(i => i.Id == instanceId);

            if (instance == null)
                throw new Exception("Instance not found");

            var transition = instance.CurrentStep.OutgoingTransitions
                .FirstOrDefault(t => t.Action == action);

            if (transition == null)
                throw new Exception($"No transition found for action {action}");

            // Update workflow instance
            instance.CurrentStepId = transition.ToStepId;
            instance.State = transition.ToState;

            // Save new step in instance history
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

            // Save performed transition
            transition.PerformedBy = userId.ToString();
            transition.Timestamp = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return instance;
        }

     
    }
}
