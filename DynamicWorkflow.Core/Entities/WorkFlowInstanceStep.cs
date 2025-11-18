using System.Text.Json.Serialization;

namespace DynamicWorkflow.Core.Entities
{
    public class WorkFlowInstanceStep:BaseEntity
    {
        public int InstanceId { get; set; }
        [JsonIgnore]
        public WorkflowInstance? Instance { get; set; }
        public int StepId { get; set; }
        [JsonIgnore]
        public WorkflowStep Step { get; set; }
        public string? PerformedByUserId { get; set; }
        public int WorkflowStatusId { get; set; }
        public WorkflowStatus WorkflowStatus { get; set; }
        public string? Comments { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
