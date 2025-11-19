namespace DynamicWorkflow.Core.Entities
{
    public  class ActionTypeEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<WorkflowStep>workflowSteps { get; set; }=new HashSet<WorkflowStep>();
        public ICollection<WorkflowTransition> WorkflowTransitions { get; set; } = new List<WorkflowTransition>();
    }
}
