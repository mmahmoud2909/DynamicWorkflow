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
        
        public ICollection<WorkflowTransition> transitions { get; set; }=new List<WorkflowTransition>();

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
