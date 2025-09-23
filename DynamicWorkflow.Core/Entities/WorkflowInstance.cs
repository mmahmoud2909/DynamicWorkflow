using DynamicWorkflow.Core.Enums;

namespace DynamicWorkflow.Core.Entities
{
    public class WorkflowInstance:BaseEntity
    {
        public int WorkflowId { get; set; }
        public Workflow? Workflow { get; set; }
        public int CurrentStepId { get; set; }
        public WorkflowStep? CurrentStep { get; set; }
        public Status State { get; set; }
        public List<WorkflowTransition> Transitions { get; set; } = new();
    }
}
