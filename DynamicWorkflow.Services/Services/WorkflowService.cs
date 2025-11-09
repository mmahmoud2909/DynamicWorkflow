using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Entities.Users;
using DynamicWorkflow.Core.Interfaces;
using DynamicWorkflow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace DynamicWorkflow.Services.Services
{
    public class WorkflowService : IWorkflowService
    {
        private readonly IWorkflow _workflow;
        private readonly ApplicationIdentityDbContext _context;

        public WorkflowService(IWorkflow workflow, ApplicationIdentityDbContext context)
        {
            _workflow = workflow;
            _context = context;
        }

        public async Task<WorkflowInstance> CreateInstanceAsync(int workflowId, ApplicationUser createdBy)
        {
            var workflow = await _context.Workflows
                .Include(w => w.Steps)
                    .ThenInclude(s => s.actionTypeEntity)
                .Include(w => w.WorkflowStatus)
                .Include(w => w.Steps)
                    .ThenInclude(s => s.workflowStatus)
                .FirstOrDefaultAsync(w => w.Id == workflowId);

            if (workflow == null)
                throw new Exception($"Workflow with ID {workflowId} not found.");

            var firstStep = workflow.Steps.OrderBy(s => s.Order).FirstOrDefault();
            if (firstStep == null)
                throw new Exception("Workflow has no defined steps.");

            // Determine initial status based on first step
            var initialStatusId = firstStep.WorkflowStatusId;

            var instance = new WorkflowInstance
            {
                WorkflowId = workflow.Id,
                Workflow = workflow,
                CurrentStepId = firstStep.Id,
                CurrentStep = firstStep,
                WorkflowStatusId = initialStatusId,
                StatusText = $"Pending on {firstStep.Name}",
                CreatedBy = createdBy.Id.ToString(),
                CreatedAt = DateTime.UtcNow
            };

            _context.WorkflowInstances.Add(instance);
            await _context.SaveChangesAsync();

            return instance;
        }

        public async Task<bool> StartWorkflowAsync(int workflowId, ApplicationUser startedBy)
        {
            var instance = await CreateInstanceAsync(workflowId, startedBy);
            return instance != null;
        }

        public async Task<bool> TransitionToStepAsync(
            int instanceId,
            int toStepId,
            string userId,
            string? comments = null)
        {
            var instance = await _context.WorkflowInstances
                .Include(i => i.CurrentStep)
                .Include(i => i.Workflow)
                    .ThenInclude(w => w.Steps)
                .Include(i => i.WorkflowStatus)
                .FirstOrDefaultAsync(i => i.Id == instanceId);

            if (instance == null)
                throw new Exception($"Workflow instance {instanceId} not found.");

            var toStep = instance.Workflow.Steps.FirstOrDefault(s => s.Id == toStepId);
            if (toStep == null)
                throw new Exception($"Target step {toStepId} not found in workflow.");

            var transition = new WorkflowTransition
            {
                WorkflowId = instance.WorkflowId,
                FromStepId = instance.CurrentStepId,
                ToStepId = toStepId,
                ActionTypeEntityId = 1,
                FromStatusId = instance.WorkflowStatusId,
                ToStatusId = toStep.WorkflowStatusId,
                Timestamp = DateTime.UtcNow,
                PerformedBy = userId,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            instance.CurrentStepId = toStepId;
            instance.CurrentStep = toStep;
            instance.WorkflowStatusId = toStep.WorkflowStatusId;
            instance.StatusText = $"Transitioned to {toStep.Name}";
            instance.UpdatedBy = userId;
            instance.UpdatedAt = DateTime.UtcNow;

            toStep.PerformedBy = userId;
            toStep.UpdatedBy = userId;
            toStep.UpdatedAt = DateTime.UtcNow;

            _context.WorkflowTransitions.Add(transition);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CancelWorkflowAsync(int instanceId, string userId, string? reason = null)
        {
            var instance = await _context.WorkflowInstances
                .Include(i => i.Workflow)
                .FirstOrDefaultAsync(i => i.Id == instanceId);

            if (instance == null)
                throw new Exception($"Workflow instance {instanceId} not found.");

            var cancelledStatus = await _context.WorkflowStatuses
                .FirstOrDefaultAsync(ws => ws.Name == "Cancelled");

            if (cancelledStatus == null)
                throw new Exception("Cancelled status not found in system.");

            instance.WorkflowStatusId = cancelledStatus.Id;
            instance.StatusText = reason ?? "Workflow cancelled";
            instance.UpdatedBy = userId;
            instance.UpdatedAt = DateTime.UtcNow;

            var transition = new WorkflowTransition
            {
                WorkflowId = instance.WorkflowId,
                FromStepId = instance.CurrentStepId,
                ActionTypeEntityId = await GetActionTypeIdByNameAsync("Cancel"),
                FromStatusId = instance.WorkflowStatusId,
                ToStatusId = cancelledStatus.Id,
                Timestamp = DateTime.UtcNow,
                PerformedBy = userId,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.WorkflowTransitions.Add(transition);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RestartWorkflowAsync(int instanceId, string userId)
        {
            var instance = await _context.WorkflowInstances
                .Include(i => i.Workflow)
                    .ThenInclude(w => w.Steps)
                .FirstOrDefaultAsync(i => i.Id == instanceId);

            if (instance == null)
                throw new Exception($"Workflow instance {instanceId} not found.");

            var firstStep = instance.Workflow.Steps.OrderBy(s => s.Order).FirstOrDefault();
            if (firstStep == null)
                throw new Exception("Workflow has no defined steps.");

            var inProgressStatus = await _context.WorkflowStatuses
                .FirstOrDefaultAsync(ws => ws.Name == "InProgress");

            if (inProgressStatus == null)
                throw new Exception("InProgress status not found in system.");

            var transition = new WorkflowTransition
            {
                WorkflowId = instance.WorkflowId,
                FromStepId = instance.CurrentStepId,
                ToStepId = firstStep.Id,
                ActionTypeEntityId = await GetActionTypeIdByNameAsync("Restart"),
                FromStatusId = instance.WorkflowStatusId,
                ToStatusId = firstStep.WorkflowStatusId,
                Timestamp = DateTime.UtcNow,
                PerformedBy = userId,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            instance.CurrentStepId = firstStep.Id;
            instance.CurrentStep = firstStep;
            instance.WorkflowStatusId = firstStep.WorkflowStatusId;
            instance.StatusText = $"Restarted - Pending on {firstStep.Name}";
            instance.UpdatedBy = userId;
            instance.UpdatedAt = DateTime.UtcNow;

            _context.WorkflowTransitions.Add(transition);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Workflow> SaveWorkflowAsync(Workflow workflow, string userId)
        {
            if (workflow.Id == 0)
            {
                workflow.CreatedBy = userId;
                workflow.CreatedAt = DateTime.UtcNow;
                _context.Workflows.Add(workflow);
            }
            else
            {
                workflow.UpdatedBy = userId;
                workflow.UpdatedAt = DateTime.UtcNow;
                _context.Workflows.Update(workflow);
            }

            await _context.SaveChangesAsync();
            return workflow;
        }

        public async Task<List<Workflow>> GetAllWorkflowsAsync()
        {
            return await _context.Workflows
                .Include(w => w.WorkflowStatus)
                .Include(w => w.Steps)
                    .ThenInclude(s => s.workflowStatus)
                .Include(w => w.Steps)
                    .ThenInclude(s => s.actionTypeEntity)
                .Include(w => w.Steps)
                    .ThenInclude(s => s.appRole)
                .OrderBy(w => w.Order)
                .ToListAsync();
        }

        public async Task<Workflow?> GetWorkflowByIdAsync(int id)
        {
            return await _context.Workflows
                .Include(w => w.WorkflowStatus)
                .Include(w => w.Steps)
                    .ThenInclude(s => s.workflowStatus)
                .Include(w => w.Steps)
                    .ThenInclude(s => s.actionTypeEntity)
                .Include(w => w.Steps)
                    .ThenInclude(s => s.appRole)
                .Include(w => w.Transitions)
                .Include(w => w.Instances)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<WorkflowInstance?> GetInstanceByIdAsync(int id)
        {
            return await _context.WorkflowInstances
                .Include(i => i.Workflow)
                    .ThenInclude(w => w.Steps)
                .Include(i => i.CurrentStep)
                .Include(i => i.Transitions)
                .Include(i => i.WorkflowStatus)
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

            if (!allWorkflows.Any())
                return false;

            var completedStatus = await _context.WorkflowStatuses
                .FirstOrDefaultAsync(ws => ws.Name == "Completed");

            if (completedStatus == null)
                return false;

            foreach (var workflow in allWorkflows)
            {
                var hasCompletedInstance = await _context.WorkflowInstances
                    .AnyAsync(i => i.WorkflowId == workflow.Id && i.WorkflowStatusId == completedStatus.Id);
                if (!hasCompletedInstance)
                    return false;
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

            var activeStatuses = await _context.WorkflowStatuses
                .Where(ws => ws.Name == "InProgress" || ws.Name == "Pending")
                .Select(ws => ws.Id)
                .ToListAsync();

            return await _context.WorkflowInstances
                .Include(i => i.Workflow)
                    .ThenInclude(w => w.Steps)
                .Include(i => i.CurrentStep)
                .Include(i => i.Transitions)
                .Include(i => i.WorkflowStatus)
                .Where(i => allWorkflows.Contains(i.WorkflowId) && activeStatuses.Contains(i.WorkflowStatusId))
                .OrderBy(i => i.Workflow.Order)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteWorkflowAsync(int id)
        {
            var workflow = await _context.Workflows
                .Include(w => w.Instances)
                .Include(w => w.Steps)
                .Include(w => w.Transitions)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workflow == null)
                return false;

            var activeStatuses = await _context.WorkflowStatuses
                .Where(ws => ws.Name == "InProgress" || ws.Name == "Pending")
                .Select(ws => ws.Id)
                .ToListAsync();

            var hasActiveInstances = workflow.Instances.Any(i => activeStatuses.Contains(i.WorkflowStatusId));
            if (hasActiveInstances)
                throw new InvalidOperationException("Cannot delete workflow with active instances.");

            _context.Workflows.Remove(workflow);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<int> GetActionTypeIdByNameAsync(string actionName)
        {
            var actionType = await _context.ActionTypes
                .FirstOrDefaultAsync(a => a.Name == actionName);

            return actionType?.Id ?? 1; // Default to 1 if not found
        }

        private async Task<int> GetStatusIdByNameAsync(string statusName)
        {
            var status = await _context.WorkflowStatuses
                .FirstOrDefaultAsync(ws => ws.Name == statusName);

            return status?.Id ?? 1; // Default to 1 if not found
        }
    }
}