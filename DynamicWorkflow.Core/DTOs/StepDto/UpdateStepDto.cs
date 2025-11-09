namespace DynamicWorkflow.Core.DTOs.StepDto
{
    public class UpdateStepDto
    {
        public string? StepName { get; set; }
        public string? Comments { get; set; }
        public int? AppRoleId { get; set; }
        public bool? IsEndStep { get; set; }
        public int? ActionTypeEntityId { get; set; }
        public int? WorkflowStatusId { get; set; }
        public int? Order { get; set; }
    }
}
