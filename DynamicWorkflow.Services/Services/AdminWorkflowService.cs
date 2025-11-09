using DynamicWorkflow.Core.DTOs.StepDto;
using DynamicWorkflow.Core.DTOs.Transition;
using DynamicWorkflow.Core.DTOs.Workflow;
using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Interfaces;
using DynamicWorkflow.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DynamicWorkflow.Services.Services
{
    public class AdminWorkflowService : IAdminWorkflowService
    {
        private readonly ApplicationIdentityDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminWorkflowService(ApplicationIdentityDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User not authenticated");
        }

        public async Task<List<WorkflowDto>> GetAllWorkflowsAsync()
        {
            var workflows = await _db.Workflows
                .Include(w => w.WorkflowStatus)
                .Include(w => w.Steps)
                    .ThenInclude(s => s.workflowStatus)
                .Include(w => w.Steps)
                    .ThenInclude(s => s.actionTypeEntity)
                .Include(w => w.Steps)
                    .ThenInclude(s => s.appRole)
                .OrderBy(w => w.Order)
                .ToListAsync();

            return workflows.Select(w => new WorkflowDto(
                w.Id,
                w.Name,
                w.Description,
                w.WorkflowStatusId,
                w.WorkflowStatus?.Name,
                w.Order,
                w.Steps.Select(s => new WorkflowStepDto(
                    s.Id,
                    s.Name,
                    s.Comments,
                    s.WorkflowStatusId,
                    s.workflowStatus?.Name,
                    s.ActionTypeEntityId,
                    s.actionTypeEntity?.Name,
                    s.isEndStep,
                    s.AppRoleId,
                    s.appRole?.Name,
                    s.WorkflowId
                )).ToList()
            )).ToList();
        }

        public async Task<WorkflowDto?> GetWorkflowByIdAsync(int id)
        {
            var workflow = await _db.Workflows
                .Include(w => w.WorkflowStatus)
                .Include(w => w.Steps)
                    .ThenInclude(s => s.workflowStatus)
                .Include(w => w.Steps)
                    .ThenInclude(s => s.actionTypeEntity)
                .Include(w => w.Steps)
                    .ThenInclude(s => s.appRole)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workflow == null) return null;

            return new WorkflowDto(
                workflow.Id,
                workflow.Name,
                workflow.Description,
                workflow.WorkflowStatusId,
                workflow.WorkflowStatus?.Name,
                workflow.Order,
                workflow.Steps.Select(s => new WorkflowStepDto(
                    s.Id,
                    s.Name,
                    s.Comments,
                    s.WorkflowStatusId,
                    s.workflowStatus?.Name,
                    s.ActionTypeEntityId,
                    s.actionTypeEntity?.Name,
                    s.isEndStep,
                    s.AppRoleId,
                    s.appRole?.Name,
                    s.WorkflowId
                )).ToList()
            );
        }

        public async Task<int> CreateWorkflowAsync(CreateWorkflowDto dto)
        {
            var userId = GetCurrentUserId();

            var workflow = new Workflow
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            _db.Workflows.Add(workflow);
            await _db.SaveChangesAsync();
            return workflow.Id;
        }

        public async Task UpdateWorkflowAsync(int id, CreateWorkflowDto dto)
        {
            var userId = GetCurrentUserId();
            var workflow = await _db.Workflows.FindAsync(id)
                ?? throw new KeyNotFoundException("Workflow not found");

            workflow.Name = dto.Name;
            workflow.Description = dto.Description;
            workflow.UpdatedBy = userId;
            workflow.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }

        public async Task DeleteWorkflowAsync(int id)
        {
            var workflow = await _db.Workflows.FindAsync(id)
                ?? throw new KeyNotFoundException("Workflow not found");

            _db.Workflows.Remove(workflow);
            await _db.SaveChangesAsync();
        }

        public async Task<int> AddStepAsync(int workflowId, CreateStepDto dto)
        {
            var userId = GetCurrentUserId();
            var workflow = await _db.Workflows.FindAsync(workflowId)
                ?? throw new KeyNotFoundException("Workflow not found");

            var step = new WorkflowStep
            {
                Name = dto.StepName,
                Comments = dto.Comments,
                ActionTypeEntityId = dto.ActionTypeEntityId,
                WorkflowStatusId = dto.WorkflowStatusId,
                isEndStep = dto.IsEndStep,
                AppRoleId = dto.AppRoleId,
                WorkflowId = workflowId,
                Order = dto.Order,
                PerformedBy = userId,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            _db.WorkflowSteps.Add(step);
            await _db.SaveChangesAsync();
            return step.Id;
        }

        public async Task UpdateStepAsync(int stepId, UpdateStepDto dto)
        {
            var userId = GetCurrentUserId();
            var step = await _db.WorkflowSteps.FindAsync(stepId)
                ?? throw new KeyNotFoundException("Step not found");

            if (!string.IsNullOrEmpty(dto.StepName)) step.Name = dto.StepName;
            if (dto.Comments != null) step.Comments = dto.Comments;
            if (dto.AppRoleId.HasValue) step.AppRoleId = dto.AppRoleId.Value;
            if (dto.IsEndStep.HasValue) step.isEndStep = dto.IsEndStep.Value;
            if (dto.ActionTypeEntityId.HasValue) step.ActionTypeEntityId = dto.ActionTypeEntityId.Value;
            if (dto.WorkflowStatusId.HasValue) step.WorkflowStatusId = dto.WorkflowStatusId.Value;
            if (dto.Order.HasValue) step.Order = dto.Order.Value;

            step.PerformedBy = userId;
            step.UpdatedBy = userId;
            step.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }

        public async Task DeleteStepAsync(int stepId)
        {
            var step = await _db.WorkflowSteps.FindAsync(stepId)
                ?? throw new KeyNotFoundException("Step not found");

            var hasTransitions = await _db.WorkflowTransitions
                .AnyAsync(t => t.FromStepId == stepId || t.ToStepId == stepId);

            if (hasTransitions)
                throw new InvalidOperationException("Step used by transitions - remove transitions first.");

            _db.WorkflowSteps.Remove(step);
            await _db.SaveChangesAsync();
        }

        public async Task<int> AddTransitionAsync(int workflowId, CreateTransitionDto dto)
        {
            var userId = GetCurrentUserId();

            var fromStep = await _db.WorkflowSteps.FindAsync(dto.FromStepId)
                ?? throw new KeyNotFoundException("From step not found");

            WorkflowStep toStep = null;
            if (dto.ToStepId.HasValue)
            {
                toStep = await _db.WorkflowSteps.FindAsync(dto.ToStepId.Value)
                    ?? throw new KeyNotFoundException("To step not found");
            }

            if (fromStep.WorkflowId != workflowId || (toStep != null && toStep.WorkflowId != workflowId))
                throw new InvalidOperationException("Steps must belong to same workflow.");

            var transition = new WorkflowTransition
            {
                FromStepId = dto.FromStepId,
                ToStepId = dto.ToStepId,
                WorkflowId = workflowId,
                ActionTypeEntityId = dto.ActionTypeEntityId,
                FromStatusId = dto.FromStatusId,
                ToStatusId = dto.ToStatusId,
                Timestamp = DateTime.UtcNow,
                PerformedBy = userId,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            _db.WorkflowTransitions.Add(transition);
            await _db.SaveChangesAsync();
            return transition.Id;
        }

        public async Task DeleteTransitionAsync(int transitionId)
        {
            var transition = await _db.WorkflowTransitions.FindAsync(transitionId)
                ?? throw new KeyNotFoundException("Transition not found");

            _db.WorkflowTransitions.Remove(transition);
            await _db.SaveChangesAsync();
        }
    }
}