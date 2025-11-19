using DynamicWorkflow.Core.DTOs.StepDto;
using DynamicWorkflow.Core.DTOs.Transition;
using DynamicWorkflow.Core.DTOs.Workflow;

namespace DynamicWorkflow.Core.Interfaces
{
    public interface IAdminWorkflowService
    {
        Task<List<WorkflowDto>> GetAllWorkflowsAsync();
        Task<WorkflowDto?> GetWorkflowByIdAsync(int id);
        Task<int> CreateWorkflowAsync(CreateWorkflowDto dto);
        Task UpdateWorkflowAsync(int id, CreateWorkflowDto dto);
        Task DeleteWorkflowAsync(int id);

        Task<int> AddStepAsync(int workflowId, CreateStepDto dto);
        Task UpdateStepAsync(int stepId, UpdateStepDto dto);
        Task DeleteStepAsync(int stepId);

        Task<int> AddTransitionAsync(int workflowId, CreateTransitionDto dto);
        Task DeleteTransitionAsync(int transitionId);
    }
}
