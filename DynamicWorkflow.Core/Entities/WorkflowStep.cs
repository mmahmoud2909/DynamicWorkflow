using DynamicWorkflow.Core.Enums;
using System.Text.Json.Serialization;

namespace DynamicWorkflow.Core.Entities
{
	public class WorkflowStep : BaseEntity
	{
		public string Name { get; set; }    // e.g. "Manager Approval"
        public string? Comments { get; set; } = null;
        public int Order { get; set; }      // execution order
        public Status stepStatus { get; set; } = Status.Pending;//enum will convert to table 
		public int WorkflowStatusId {  get; set; }
		public WorkflowStatus workflowStatus { get; set; }
		public int ActionTypeEntityId { get; set; }
		public ActionTypeEntity actionTypeEntity { get; set; }
		public ActionType stepActionTypes { get; set; }//will convert to table
		public bool isEndStep {  get; set; }
		public Roles AssignedRole { get; set; }//enum will convert it into tables 
		public int AppRoleId {  get; set; }
		public AppRole appRole { get; set; }

		[JsonIgnore]
		public Workflow workflow { get; set; }
		public int WorkflowId {  get; set; }
        public Guid? AssignedUserId { get; set; }//TO Assign User to determined Step 

        public ICollection<StepRole> Roles { get; set; } = new List<StepRole>();
        public ICollection<WorkflowTransition> IncomingTransitions { get; set; }=new List<WorkflowTransition>();
		public ICollection<WorkflowTransition> OutgoingTransitions { get; set; } = new List<WorkflowTransition>();
		public ICollection<WorkFlowInstanceStep> InstanceSteps { get; set; } = new List<WorkFlowInstanceStep>();
        public string? Condition { get; set; }
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
