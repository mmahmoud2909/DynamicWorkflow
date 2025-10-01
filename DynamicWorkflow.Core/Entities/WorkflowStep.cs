using DynamicWorkflow.Core.Enums;

namespace DynamicWorkflow.Core.Entities
{
	public class WorkflowStep : BaseEntity
	{
		public string Name { get; set; }    // e.g. "Manager Approval"
        public string? Comments { get; set; } = null;
        public int Order { get; set; }      // execution order
        public Status stepStatus { get; set; } = Status.Pending;
		public ActionType stepActionTypes { get; set; }
		public bool isEndStep {  get; set; }
		public Roles AssignedRole { get; set; }
		public Workflow workflow { get; set; }
		public int WorkflowId {  get; set; }

        public ICollection<StepRole> Roles { get; set; } = new List<StepRole>();
        public ICollection<WorkflowTransition> IncomingTransitions { get; set; }=new List<WorkflowTransition>();
		public ICollection<WorkflowTransition> OutgoingTransitions { get; set; } = new List<WorkflowTransition>();
		public ICollection<WorkFlowInstanceStep> InstanceSteps { get; set; } = new List<WorkFlowInstanceStep>();

        public WorkflowStep() { }
		public WorkflowStep(string stepName, string comments, Status stepStatus, ActionType stepActionTypes)
		{
			this.Name = stepName;
			this.Comments = comments;
			this.stepStatus = Status.Pending;
			this.stepActionTypes = stepActionTypes;
		}
	}
}
