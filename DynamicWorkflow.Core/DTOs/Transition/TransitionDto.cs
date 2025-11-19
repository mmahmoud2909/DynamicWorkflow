using DynamicWorkflow.Core.Entities;

namespace DynamicWorkflow.Core.DTOs.Transition
{
    public class TransitionDto
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string PerformedBy { get; set; }
        public int FromStepId { get; set; }
        public int ToStepId { get; set; }
        public ActionTypeEntity Action { get; set; }
        public WorkflowStatus FromState { get; set; }
        public WorkflowStatus ToState { get; set; }
    }
}
