namespace DynamicWorkflow.Core.Entities
{
    public  class ActionTypeEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<WorkflowInstance> WorkflowInstances { get; set; } = new List<WorkflowInstance>();
        public ICollection<WorkFlowInstanceStep> WorkflowInstanceSteps { get; set; } = new List<WorkFlowInstanceStep>();
        public ICollection<WorkflowStep>workflowSteps { get; set; }=new HashSet<WorkflowStep>();
        public ICollection<WorkflowTransition> WorkflowTransitions { get; set; } = new List<WorkflowTransition>();
    }
}
