
namespace DynamicWorkflow.Core.Entities
{
    public class DocumentRequirement
    {
        public int Id { get; set; }
        public string DocumentName { get; set; }
        public int StepId { get; set; }
        public WorkflowStep Step { get; set; }
    }
}