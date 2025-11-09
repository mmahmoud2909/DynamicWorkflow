using System.Text.Json.Serialization;

namespace DynamicWorkflow.Core.Entities
{
    public class WorkflowInstance:BaseEntity
    {
        public int WorkflowId { get; set; }
        [JsonIgnore]
        public Workflow? Workflow { get; set; }
        public int CurrentStepId { get; set; }
        public WorkflowStep? CurrentStep { get; set; }
        public int WorkflowStatusId { get; set; }
        public WorkflowStatus WorkflowStatus { get; set; }
        public string? StatusText { get; set; }
        public List<WorkflowTransition> Transitions { get; set; } = new();
        public string? PerformedBy { get; set; }
    }
}
