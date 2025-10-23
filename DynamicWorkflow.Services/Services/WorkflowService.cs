using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Enums;
using DynamicWorkflow.Core.Interfaces;

namespace DynamicWorkflow.Services.Services
{
    public class WorkflowService : IWorkflowService
    {
        private readonly IWorkflow _workflow;
        private readonly IStateMachineFactory _factory;

        public WorkflowService(IWorkflow workflow, IStateMachineFactory factory)
        {
            _workflow = workflow;
            _factory = factory;
        }

        public async Task<WorkflowInstance> CreateWorkflow(int workflowId)
        {
            var instance = new WorkflowInstance
            {
                WorkflowId = workflowId,
                State = Status.InProgress,
                CurrentStepId = 0,
                Transitions = new List<WorkflowTransition>(),
                WorkflowStatusId = (int)Status.InProgress
            };

            await _workflow.AddAsync(instance);
            return instance;
        }

        public async Task TriggerAsync(int workflowId, ActionType action, string user)
        {
            var instance = await _workflow.GetByIdAsync(workflowId)
                          ?? throw new KeyNotFoundException();

            var sm = _factory.Create(instance, newState =>
            {
                instance.Transitions.Add(new WorkflowTransition
                {
                    Action = action,
                    ActionTypeEntityId = (int)action,
                    FromState = instance.State,
                    ToState = newState,
                    Timestamp = DateTime.UtcNow,
                    PerformedBy = user,
                    WorkflowId = instance.WorkflowId,
                    FromStepId = instance.CurrentStepId,
                    ToStepId = instance.CurrentStepId // later update to actual next step
                });
            });

            sm.Fire(action);  // or FireAsync for async entry/exit actions

            await _workflow.SaveAsync(instance);
        }

        public async Task<WorkflowInstance> FireTrigger(int instanceId, ActionType action, string user)
        {
            var instance = await _workflow.GetByIdAsync(instanceId);
            if (instance == null) throw new Exception("Workflow instance not found");

            var stateMachine = _factory.Create(instance, newState =>
            {
                instance.Transitions.Add(new WorkflowTransition
                {
                    Action = action,
                    ActionTypeEntityId = (int)action,
                    FromState = instance.State,
                    ToState = newState,
                    Timestamp = DateTime.UtcNow,
                    PerformedBy = user,
                    WorkflowId = instance.WorkflowId,
                    FromStepId = instance.CurrentStepId,
                    ToStepId = instance.CurrentStepId // later you update to actual "next step"
                });
            });

            stateMachine.Fire(action);

            await _workflow.UpdateAsync(instance);
            return instance;
        }
    }
}
    
