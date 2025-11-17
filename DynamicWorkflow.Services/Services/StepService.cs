using DynamicWorkflow.Core.DTOs.StepDto;
using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Entities.Users;
using DynamicWorkflow.Core.Interfaces;
using DynamicWorkflow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace DynamicWorkflow.Services.Services
{
    public class StepService : IWorkflow, IStepService
    {
        private readonly ApplicationIdentityDbContext _context;

        public StepService(ApplicationIdentityDbContext context)
        {
            _context = context;
        }
        public async Task MakeActionAsync(Workflow workflow, int stepId, int actionTypeEntityId, ApplicationUser currentUser)
        {
            var step = workflow.Steps.FirstOrDefault(s => s.Id == stepId);
            if (step == null)
                throw new Exception($"Step with ID {stepId} not found.");

            var actionTypeEntity = await _context.ActionTypes.FindAsync(actionTypeEntityId);
            if (actionTypeEntity == null)
                throw new Exception($"Action type with ID {actionTypeEntityId} not found.");
            
            if (!await CanUserPerformStepAsync(stepId, currentUser))
                throw new UnauthorizedAccessException($"User is not authorized to perform this step.");

            step.PerformedBy = currentUser.Id.ToString();
            step.UpdatedBy = currentUser.Id.ToString();
            step.UpdatedAt = DateTime.UtcNow;

            var nextStep = workflow.Steps.OrderBy(s => s.Order).FirstOrDefault(s => s.Order > step.Order);

            var transition = new WorkflowTransition
            {
                WorkflowId = workflow.Id,
                FromStepId = step.Id,
                ToStepId = nextStep?.Id,
                ActionTypeEntityId = actionTypeEntityId,
                FromStatusId = step.WorkflowStatusId,
                ToStatusId = nextStep?.WorkflowStatusId,
                Timestamp = DateTime.UtcNow,
                PerformedBy = currentUser.Id.ToString(),
                CreatedBy = currentUser.Id.ToString(),
                CreatedAt = DateTime.UtcNow
            };

            _context.WorkflowTransitions.Add(transition);

            if (nextStep == null)
            {
                var completedStatus = await _context.WorkflowStatuses
                    .FirstOrDefaultAsync(ws => ws.Name == "Completed");
                if (completedStatus != null)
                {
                    workflow.WorkflowStatusId = completedStatus.Id;
                    workflow.UpdatedBy = currentUser.Id.ToString();
                    workflow.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<StepDto>> GetAllStepsAsync(int workflowId)
        {
            return await _context.WorkflowSteps
                .Where(s => s.WorkflowId == workflowId)
                .OrderBy(s => s.Order)
                .Select(s => new StepDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Comments = s.Comments,
                    Order = s.Order,
                    IsEndStep = s.isEndStep,

                    WorkflowStatusId = s.WorkflowStatusId,
                    WorkflowStatus = s.workflowStatus == null ? null : new WorkflowStatusDto
                    {
                        Id = s.workflowStatus.Id,
                        Name = s.workflowStatus.Name,
                        Description = s.workflowStatus.Description
                    },

                    ActionTypeEntityId = s.ActionTypeEntityId,
                    ActionTypeEntity = s.actionTypeEntity == null ? null : new ActionTypeDto
                    {
                        Id = s.actionTypeEntity.Id,
                        Name = s.actionTypeEntity.Name,
                        Description = s.actionTypeEntity.Description
                    },

                    AppRoleId = s.AppRoleId,
                    AppRole = s.appRole == null ? null : new AppRoleDto
                    {
                        Id = s.appRole.Id,
                        Name = s.appRole.Name,
                        Description = s.appRole.Description
                    }
                })
                .ToListAsync();
        }

        public async Task<WorkflowStep?> GetStepByIdAsync(int stepId)
        {
            return await _context.WorkflowSteps
                .Include(s => s.workflowStatus)
                .Include(s => s.actionTypeEntity)
                .Include(s => s.appRole)
                .Include(s => s.workflow)
                .FirstOrDefaultAsync(s => s.Id == stepId);
        }
        
        public async Task<bool> CanUserPerformStepAsync(int stepId, ApplicationUser user)
        {
            var step = await _context.WorkflowSteps
                .Include(s => s.appRole)
                .FirstOrDefaultAsync(s => s.Id == stepId);

            if (step == null)
                return false;

            var userRoles = await (from ur in _context.UserRoles
                                   join r in _context.Roles on ur.RoleId equals r.Id
                                   where ur.UserId == user.Id
                                   select r.Name).ToListAsync();

            var requiredRole = step.appRole?.Name;
            if (requiredRole != null && (userRoles.Contains(requiredRole) || userRoles.Contains("Admin")))
                return true;

            if (step.AssignedUserId.HasValue && step.AssignedUserId.Value == user.Id)
                return true;

            return false;
        }

        public async Task<WorkflowStep> CreateStepAsync(WorkflowStep step, string userId)
        {
            step.CreatedBy = userId;
            step.CreatedAt = DateTime.UtcNow;

            _context.WorkflowSteps.Add(step);
            await _context.SaveChangesAsync();

            return step;
        }

        public async Task UpdateStepAsync(WorkflowStep step, string userId)
        {
            step.UpdatedBy = userId;
            step.UpdatedAt = DateTime.UtcNow;

            _context.WorkflowSteps.Update(step);
            await _context.SaveChangesAsync();
        }

        public async Task<WorkflowInstance?> GetByIdAsync(int id)
        {
            return await _context.WorkflowInstances
                .Include(i => i.Workflow)
                .Include(i => i.CurrentStep)
                .Include(i => i.Transitions)
                .Include(i => i.WorkflowStatus)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task SaveAsync(WorkflowInstance instance)
        {
            if (instance.Id == 0)
            {
                await AddAsync(instance);
            }
            else
            {
                await UpdateAsync(instance);
            }
        }

        public async Task AddAsync(WorkflowInstance instance)
        {
            _context.WorkflowInstances.Add(instance);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(WorkflowInstance instance)
        {
            _context.WorkflowInstances.Update(instance);
            await _context.SaveChangesAsync();
        }
    }
}