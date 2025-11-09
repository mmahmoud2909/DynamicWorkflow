using System.Text.Json.Serialization;

namespace DynamicWorkflow.Core.Entities
{
	public class WorkflowStep : BaseEntity
	{
		public string Name { get; set; }
        public string? Comments { get; set; } = null;
        public int Order { get; set; }
		public int WorkflowStatusId { get; set; }
		public WorkflowStatus workflowStatus { get; set; }
		public int ActionTypeEntityId { get; set; }
		public ActionTypeEntity actionTypeEntity { get; set; }
		public bool isEndStep {  get; set; }
		public int AppRoleId {  get; set; }
		public AppRole appRole { get; set; }
		[JsonIgnore]
		public Workflow workflow { get; set; }
		public int WorkflowId {  get; set; }
        public Guid? AssignedUserId { get; set; }
        public string? PerformedBy { get; set; }
        public ICollection<WorkflowTransition> IncomingTransitions { get; set; } = new List<WorkflowTransition>();
		public ICollection<WorkflowTransition> OutgoingTransitions { get; set; } = new List<WorkflowTransition>();
		public ICollection<WorkFlowInstanceStep> InstanceSteps { get; set; } = new List<WorkFlowInstanceStep>();
        public WorkflowStep() { }
	}
}
