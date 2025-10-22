namespace DynamicWorkflow.Core.Entities
{
    public  class ActionTypeEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // e.g. "Accept", "Reject", "Hold"
        public string? Description { get; set; }

        // 🔹 Navigation Properties
        public ICollection<WorkflowInstance> WorkflowInstances { get; set; } = new List<WorkflowInstance>();
        public ICollection<WorkFlowInstanceStep> WorkflowInstanceSteps { get; set; } = new List<WorkFlowInstanceStep>();
        public ICollection<WorkflowInstanceAction>WorkflowInstanceActions { get; set; }=new List<WorkflowInstanceAction>();
        public ICollection<WorkflowStep>workflowSteps { get; set; }=new HashSet<WorkflowStep>();
        public ICollection<WorkflowTransition> WorkflowTransitions { get; set; } = new List<WorkflowTransition>();
    }
}
