using DynamicWorkflow.Core.Entities.Users;
using DynamicWorkflow.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWorkflow.Core.Entities
{
	public class WorkflowStep : BaseEntity
	{
		public string stepName { get; set; }
		public string? comments { get; set; } = null;
		public Status stepStatus { get; set; } = Status.Pending;
		public ActionType stepActionTypes { get; set; }
		public bool isEndStep {  get; set; }
		public Roles AssignedRole { get; set; }
		public Workflow workflow { get; set; }
		public int WorkflowId {  get; set; }
		
		public ICollection<WorkflowTransition> IncomingTransitions { get; set; }=new List<WorkflowTransition>();
		public ICollection<WorkflowTransition> OutgoingTransitions { get; set; } = new List<WorkflowTransition>();
		public ICollection<WorkFlowInstanceStep> InstanceSteps { get; set; } = new List<WorkFlowInstanceStep>();



        public WorkflowStep() { }
		public WorkflowStep(string stepName, string comments, Status stepStatus, ActionType stepActionTypes)
		{
			this.stepName = stepName;
			this.comments = comments;
			this.stepStatus = Status.Pending;
			this.stepActionTypes = stepActionTypes;
		}
	}
}
