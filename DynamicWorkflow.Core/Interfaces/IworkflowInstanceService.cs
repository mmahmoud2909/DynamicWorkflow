using DynamicWorkflow.Core.Entities;
namespace DynamicWorkflow.Core.Interfaces
{
    public interface IworkflowInstanceService
    {
        Task<WorkflowInstance> CreateInstanceAsync(int workflowId, Guid userId);
        Task<IList<WorkflowInstance>> GetInstancesForUserAsync(Guid userId);
        Task<WorkflowInstance> MoveToNextStepAsync(int instanceId, Guid userId, ActionTypeEntity action, string? comments = null);
        Task<WorkflowInstance?> GetInstanceByIdAsync(int instanceId);
    }
}
