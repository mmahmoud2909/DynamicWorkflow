using DynamicWorkflow.Core.DTOs.StepDto;
using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Entities.Users;

namespace DynamicWorkflow.Core.Interfaces
{
    public interface IStepService
    {
        Task MakeActionAsync(Workflow workflow, int stepId, int actionTypeEntityId, ApplicationUser currentUser);
        Task<List<StepDto>> GetAllStepsAsync(int workflowId);
        Task<WorkflowStep?> GetStepByIdAsync(int stepId);
        Task<bool> CanUserPerformStepAsync(int stepId, ApplicationUser user);
        Task<WorkflowStep> CreateStepAsync(WorkflowStep step, string userId);
        Task UpdateStepAsync(WorkflowStep step, string userId);
    }
}
