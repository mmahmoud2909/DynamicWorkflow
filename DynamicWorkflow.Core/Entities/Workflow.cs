using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWorkflow.Core.Entities
{
    public class Workflow : BaseEntity
    {
        public string name { get; set; }
        public string description { get; set; }
        public ICollection<WorkflowStep> steps { get; set; } = new List<WorkflowStep>();
        public ICollection<WorkflowInstance> Instances {  get; set; }= new HashSet<WorkflowInstance>();
        public ICollection<WorkflowTransition> Transitions { get; set; } = new HashSet<WorkflowTransition>();
        public Workflow() { }
    }
}
