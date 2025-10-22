using System.Text.Json.Serialization;
namespace DynamicWorkflow.Core.Entities
{
    public class WorkflowInstanceAction : BaseEntity
    {
        public int WorkflowInstanceId { get; set; }
        [JsonIgnore]
        public WorkflowInstance WorkflowInstance { get; set; }

        public Guid PerformedByUserId { get; set; }

        public int ActionTypeEntityId { get; set; }
        public ActionTypeEntity Action { get; set; }

        public string? Comments { get; set; }
        public DateTime PerformedAt { get; set; }

        public int WorkFlowInstanceStepId { get; set; }
        public WorkFlowInstanceStep WorkFlowInstanceStep { get; set; }

        public int WorkflowStepId { get; set; }

        public WorkflowStatus workflowStatus { get; set; }
        public int WorkflowStatusId { get; set; }
    }
}