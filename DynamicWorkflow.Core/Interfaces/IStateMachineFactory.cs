using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Enums;
using Stateless;

namespace DynamicWorkflow.Core.Interfaces
{
    public interface IStateMachineFactory
    {
        StateMachine<Status, ActionType> Create(WorkflowInstance instance, Action<Status> onStateChanged);
    }
}
