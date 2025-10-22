
using DynamicWorkflow.Core.Enums;

namespace DynamicWorkflow.Core.Entities
{
    public class Workflow : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentWorkflowId { get; set; } // nullable because the root WF has no parent
        public int Order { get; set; }                        // order in hierarchy
        public Status Status { get; set; } = Status.Pending;//Here I wanna Convert to Table
        public int WorkflowStatusId {  get; set; }
        public WorkflowStatus WorkflowStatus {  get; set; }
        public List<WorkflowStep> Steps { get; set; } = new List<WorkflowStep>();
        public ICollection<WorkflowInstance> Instances {  get; set; }= new HashSet<WorkflowInstance>();
        public ICollection<WorkflowTransition> Transitions { get; set; } = new HashSet<WorkflowTransition>();
        

        public Workflow() { }
    }
}
