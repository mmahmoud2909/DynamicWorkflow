namespace DynamicWorkflow.Core.Entities
{
    public class AppRole
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // e.g. "Procurement", "Planning", "CEO"
        public string? Description { get; set; }
        public ICollection<WorkflowStep> WorkflowSteps { get; set; } = new List<WorkflowStep>();

    }
}
