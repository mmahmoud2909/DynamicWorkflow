namespace DynamicWorkflow.Core.DTOs.Transition
{
    public class CreateTransitionDto
    {
        public int FromStepId { get; set; }
        public int? ToStepId { get; set; }
        public int ActionTypeEntityId { get; set; }
        public int? FromStatusId { get; set; }
        public int? ToStatusId { get; set; }
    }
}
