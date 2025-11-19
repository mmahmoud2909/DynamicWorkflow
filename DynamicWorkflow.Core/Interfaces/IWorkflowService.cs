using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Entities.Users;
namespace DynamicWorkflow.Services.Services
{
    public interface IWorkflowService
    {
        Task<Workflow> SaveWorkflowAsync(Workflow workflow, string userId);
        Task<List<Workflow>> GetAllWorkflowsAsync();
        Task<Workflow?> GetWorkflowByIdAsync(int id);
        Task<bool> DeleteWorkflowAsync(int id);
        Task<WorkflowInstance> CreateInstanceAsync(int workflowId, ApplicationUser createdBy);
        Task<WorkflowInstance?> GetInstanceByIdAsync(int id);
        Task<bool> TransitionToStepAsync(int instanceId, int toStepId, string userId, string? comments = null);
        Task<List<WorkflowInstance>> GetWorkflowChainInstancesAsync(int? parentWorkflowId);
        Task<bool> IsWorkflowChainCompletedAsync(int? parentWorkflowId);
        Task<WorkflowInstance?> GetActiveInstanceInChainAsync(int? parentWorkflowId);
        Task<bool> StartWorkflowAsync(int workflowId, ApplicationUser startedBy);
        Task<bool> CancelWorkflowAsync(int instanceId, string userId, string? reason = null);
        Task<bool> RestartWorkflowAsync(int instanceId, string userId);
    }
}