using DynamicWorkflow.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class WorkflowTransition : BaseEntity
{
    public int FromStepId { get; set; }
    public int? ToStepId { get; set; }
    public int WorkflowId { get; set; }
    public Workflow Workflow { get; set; }
    public int ActionTypeEntityId { get; set; }
    [ForeignKey(nameof(ActionTypeEntityId))]
    public ActionTypeEntity ActionTypeEntity { get; set; }
    public int? FromStatusId { get; set; }
    [ForeignKey(nameof(FromStatusId))]
    public WorkflowStatus FromStatus { get; set; }
    public int? ToStatusId { get; set; }
    [ForeignKey(nameof(ToStatusId))]
    public WorkflowStatus ToStatus { get; set; }
    [JsonIgnore]
    public WorkflowStep FromStep { get; set; }
    [JsonIgnore]
    public WorkflowStep? ToStep { get; set; }
    public DateTime Timestamp { get; set; }
    public string? PerformedBy { get; set; }
}
