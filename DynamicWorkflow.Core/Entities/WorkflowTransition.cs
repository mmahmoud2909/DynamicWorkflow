using DynamicWorkflow.Core.Enums;

namespace DynamicWorkflow.Core.Entities
{
    public class WorkflowTransition:BaseEntity
    {
        public int FromStepId { get; set; }
        public int ToStepId { get; set; }
        public Workflow workflow { get; set; }
        public int WorkflowId { get; set; }
        public WorkflowStep? FromStep { get; set; }
        public WorkflowStep? ToStep { get; set; }
        public ActionType Action {  get; set; }
        public Status FromState { get; set; }
        public Status ToState { get; set; }
        public DateTime Timestamp { get; set; }
        public string? PerformedBy { get; set; }

    }
}
