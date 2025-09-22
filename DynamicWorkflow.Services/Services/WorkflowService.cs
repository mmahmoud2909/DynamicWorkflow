using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Enums;
using DynamicWorkflow.Core.Interfaces;

namespace DynamicWorkflow.Services.Services
{
    public class WorkflowService
    {
        private readonly IWorkflow _workflow;

        public WorkflowService(IWorkflow workflow)
        {
            _workflow = workflow;
        }

        public async Task<WorkflowInstance> CreateWorkflow(string createdBy)
        {
            await _workflow.SaveAsync(instance);
            return instance;
        }

        public async Task<WorkflowInstance> CreateWorkflow(int workflowId, string createdBy)
        {
            var instance = new WorkflowInstance
            {
                WorkflowId = workflowId,
                State = Status.Created,
                CurrentStepId = 0,   // start step if you have one
                Transitions = new List<WorkflowTransition>()
            };

            await _workflow.AddAsync(instance);
            return instance;
        }

        public async Task TriggerAsync(Guid workflowId, ActionType action)
        {
            var instance = await _workflow.GetByIdAsync(workflowId)
                          ?? throw new KeyNotFoundException();

            var sm = StateMachineFactory.Create(instance);

            sm.Fire(action);  // or FireAsync for async entry/exit actions

            await _workflow.SaveAsync(instance);
        }


        public async Task<WorkflowInstance> FireTrigger(Guid id, ActionType action, string user)
        {
            var instance = await _workflow.GetByIdAsync(id);
            if (instance == null) throw new Exception("Workflow not found");

            var stateMachine = StateMachineFactory.Create(instance, newState =>
            {
                // optional: log or raise domain event
                instance.Transitions.Add(new WorkflowTransition
                {
                    Action = action,
                    FromState = instance.State,
                    ToState = newState,
                    Timestamp = DateTime.UtcNow,
                    PerformedBy = user
                });
            });

            stateMachine.Fire(action);

            await _workflow.UpdateAsync(instance);
            return instance;
        }

        public async Task<WorkflowInstance> FireTrigger(int instanceId, ActionType action, string user)
        {
            var instance = await _workflow.GetByIdAsync(instanceId);
            if (instance == null) throw new Exception("Workflow instance not found");

            var stateMachine = StateMachineFactory.Create(instance, newState =>
            {
                // You may update CurrentStep based on workflow definition
                // Example: move to next WorkflowStep entity
            });

            stateMachine.Fire(action);

            await _workflow.UpdateAsync(instance);
            return instance;
        }
    }
}
    
