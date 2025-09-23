using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Enums;
using DynamicWorkflow.Core.Interfaces;

namespace DynamicWorkflow.Services.Services
{
    public interface IWorkflowService
    {
        Task<WorkflowInstance> FireTrigger(int instanceId, ActionType action, string user);
        Task TriggerAsync(int workflowId, ActionType action, string user);
        Task<WorkflowInstance> CreateWorkflow(int workflowId);

    }
}