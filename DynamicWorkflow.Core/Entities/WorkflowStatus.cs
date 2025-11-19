namespace DynamicWorkflow.Core.Entities
{
    public class WorkflowStatus
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ICollection<WorkflowStep> WorkflowSteps { get; set; } = new List<WorkflowStep>();
        public ICollection<WorkflowTransition> WorkflowTransitions { get; set; } = new List<WorkflowTransition>();
        public ICollection<Workflow> workflows {  get; set; }= new List<Workflow>();
        public ICollection<WorkflowInstance> WorkflowInstances {  get; set; }=new List<WorkflowInstance>();
        public ICollection<WorkFlowInstanceStep> WorkflowInstanceSteps { get; set; }=new List<WorkFlowInstanceStep>();
    }
}
