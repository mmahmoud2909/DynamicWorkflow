using DynamicWorkflow.Core.Enums;

namespace DynamicWorkflow.Core.DTOs.Transition
{
    public class CreateTransitionDto
    {
        public int FromStepId { get; set; }
        public int ToStepId { get; set; }
        public ActionType Action { get; set; }
        public Status FromState { get; set; }
        public Status ToState { get; set; }
    }
}
