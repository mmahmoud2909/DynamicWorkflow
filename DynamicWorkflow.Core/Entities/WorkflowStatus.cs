namespace DynamicWorkflow.Core.Entities
{
    public class WorkflowStatus
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // e.g. "Pending", "InProgress", "Completed"
        public string? Description { get; set; }
        
        public ICollection<WorkflowInstanceAction> WorkflowInstanceActions { get; set; } = new List<WorkflowInstanceAction>();
        public ICollection<WorkflowStep> WorkflowSteps { get; set; } = new List<WorkflowStep>();
        public ICollection<WorkflowTransition> WorkflowTransitions { get; set; } = new List<WorkflowTransition>();
        public ICollection<Workflow> workflows {  get; set; }= new List<Workflow>();
        public ICollection<WorkflowInstance> WorkflowInstances {  get; set; }=new List<WorkflowInstance>();
    }
}
