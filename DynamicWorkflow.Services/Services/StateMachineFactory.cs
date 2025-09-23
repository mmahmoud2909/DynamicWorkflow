using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Enums;
using DynamicWorkflow.Core.Interfaces;
using Stateless;

namespace DynamicWorkflow.Services.Services
{
    public class StateMachineFactory : IStateMachineFactory
    {
        public StateMachine<Status, ActionType> Create(WorkflowInstance instance, Action<Status> onStateChanged)
        {
            var sm = new StateMachine<Status, ActionType>(
                () => instance.State,         // getter
                s => instance.State = s       // setter
            );

            sm.Configure(Status.Pending)
                .Permit(ActionType.Accept, Status.Completed);

            sm.Configure(Status.Pending)
                .Permit(ActionType.Accept, Status.Accepted)
                .Permit(ActionType.Reject, Status.Rejected);

            sm.Configure(Status.Accepted)
                .Permit(ActionType.Accept, Status.Completed);

            // on entry action
            sm.OnTransitioned(t =>
            {
                onStateChanged?.Invoke(t.Destination);
            });

            return sm;
        }
    }
}
