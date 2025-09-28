using DynamicWorkflow.Core.Enums;

namespace DynamicWorkflow.Core.DTOs.Transition
{
    public class TransitionDto
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string PerformedBy { get; set; }
        public int FromStepId { get; set; }
        public int ToStepId { get; set; }
        public ActionType Action { get; set; }
        public Status FromState { get; set; }
        public Status ToState { get; set; }
    }
}
