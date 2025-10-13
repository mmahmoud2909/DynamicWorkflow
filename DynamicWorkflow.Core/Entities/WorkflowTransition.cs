using DynamicWorkflow.Core.Enums;
using System.Text.Json.Serialization;

namespace DynamicWorkflow.Core.Entities
{
    public class WorkflowTransition:BaseEntity
    {
        public int FromStepId { get; set; }
        public int ?ToStepId { get; set; }
        public Workflow workflow { get; set; }
        public int WorkflowId { get; set; }
        [JsonIgnore]

        public WorkflowStep? FromStep { get; set; }
        [JsonIgnore]
        public WorkflowStep? ToStep { get; set; }
        public ActionType Action {  get; set; }
        public Status FromState { get; set; }
        public Status ToState { get; set; }
        public DateTime Timestamp { get; set; }
        public string? PerformedBy { get; set; }

    }
}
