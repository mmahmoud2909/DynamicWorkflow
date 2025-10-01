using DynamicWorkflow.Core.DTOs.StepDto;
using DynamicWorkflow.Core.DTOs.Transition;
using DynamicWorkflow.Core.DTOs.Workflow;
using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Interfaces;
using DynamicWorkflow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace DynamicWorkflow.Services.Services
{
    public class AdminWorkflowService : IAdminWorkflowService
    {
        private readonly ApplicationIdentityDbContext _db;

        public AdminWorkflowService(ApplicationIdentityDbContext db) => _db = db;

        public async Task<List<WorkflowDto>> GetAllWorkflowsAsync()
        {
            var list = await _db.Workflows
                .Include(w => w.Steps)
                .ThenInclude(s => s.InstanceSteps)
                .ToListAsync();

            return list.Select(w => new WorkflowDto(
                w.Id,
                w.Name,
                w.Description,
                w.Steps.Select(s => new WorkflowStepDto(
                    s.Id,
                    s.Name,
                    s.Comments,
                    s.stepStatus,
                    s.stepActionTypes,
                    s.isEndStep,
                    s.AssignedRole,
                    s.WorkflowId
                )).ToList()
            )).ToList();
        }

        public async Task<WorkflowDto?> GetWorkflowByIdAsync(int id)
        {
            var w = await _db.Workflows.Include(x => x.Steps).FirstOrDefaultAsync(x => x.Id == id);
            if (w == null) return null;
            return new WorkflowDto(w.Id, w.Name, w.Description, w.Steps.Select(s => new WorkflowStepDto(s.Id, s.Name, s.Comments, s.stepStatus, s.stepActionTypes, s.isEndStep, s.AssignedRole, s.WorkflowId)).ToList());
        }

        public async Task<int> CreateWorkflowAsync(CreateWorkflowDto dto)
        {
            var w = new Workflow { Name = dto.Name, Description = dto.Description };
            _db.Workflows.Add(w);
            await _db.SaveChangesAsync();
            return w.Id;
        }

        public async Task UpdateWorkflowAsync(int id, CreateWorkflowDto dto)
        {
            var w = await _db.Workflows.FindAsync(id) ?? throw new KeyNotFoundException("Workflow not found");
            w.Name = dto.Name;
            w.Description = dto.Description;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteWorkflowAsync(int id)
        {
            var w = await _db.Workflows.FindAsync(id) ?? throw new KeyNotFoundException("Workflow not found");
            _db.Workflows.Remove(w);
            await _db.SaveChangesAsync();
        }

        // Steps
        public async Task<int> AddStepAsync(int workflowId, CreateStepDto dto)
        {
            var wf = await _db.Workflows.FindAsync(workflowId) ?? throw new KeyNotFoundException("Workflow not found");
            var step = new WorkflowStep
            {
                Name = dto.stepName,
                Comments = dto.comments,
                stepActionTypes = dto.stepActionTypes,
                isEndStep = dto.isEndStep,
                AssignedRole = dto.assignedRole,
                WorkflowId = workflowId
            };
            _db.WorkflowSteps.Add(step);
            await _db.SaveChangesAsync();
            return step.Id;
        }

        public async Task UpdateStepAsync(int stepId, UpdateStepDto dto)
        {
            var step = await _db.WorkflowSteps.FindAsync(stepId) ?? throw new KeyNotFoundException("Step not found");
            if (dto.stepName is not null) step.Name = dto.stepName;
            if (dto.comments is not null) step.Comments = dto.comments;
            if (dto.assignedRole is not null) step.AssignedRole = dto.assignedRole.Value;
            if (dto.isEndStep.HasValue) step.isEndStep = dto.isEndStep.Value;
            if (dto.stepActionTypes.HasValue) step.stepActionTypes = dto.stepActionTypes.Value;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteStepAsync(int stepId)
        {
            var step = await _db.WorkflowSteps.FindAsync(stepId) ?? throw new KeyNotFoundException("Step not found");
            // optionally check for Instance steps or transitions referencing this step
            var referencing = await _db.WorkflowTransitions.AnyAsync(t => t.FromStepId == stepId || t.ToStepId == stepId);
            if (referencing) throw new InvalidOperationException("Step used by transitions - remove transitions first.");
            _db.WorkflowSteps.Remove(step);
            await _db.SaveChangesAsync();
        }

        // Transitions
        public async Task<int> AddTransitionAsync(int workflowId, CreateTransitionDto dto)
        {
            // ensure steps exist and belong to workflowId
            var from = await _db.WorkflowSteps.FindAsync(dto.FromStepId) ?? throw new KeyNotFoundException("From step not found");
            var to = await _db.WorkflowSteps.FindAsync(dto.ToStepId) ?? throw new KeyNotFoundException("To step not found");
            if (from.WorkflowId != workflowId || to.WorkflowId != workflowId) throw new InvalidOperationException("Steps must belong to same workflow.");

            var t = new WorkflowTransition
            {
                FromStepId = dto.FromStepId,
                ToStepId = dto.ToStepId,
                WorkflowId = workflowId,
                Action = dto.Action,
                FromState = dto.FromState,
                ToState = dto.ToState,
                Timestamp = DateTime.UtcNow,
                PerformedBy = "system" // admin-created; runtime transitions will set real user
            };
            _db.WorkflowTransitions.Add(t);
            await _db.SaveChangesAsync();
            return t.Id;
        }

        public async Task DeleteTransitionAsync(int transitionId)
        {
            var t = await _db.WorkflowTransitions.FindAsync(transitionId) ?? throw new KeyNotFoundException("Transition not found");
            _db.WorkflowTransitions.Remove(t);
            await _db.SaveChangesAsync();
        }
    }
}
