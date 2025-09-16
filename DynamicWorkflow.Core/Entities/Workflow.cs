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
        public ICollection<WorkflowStep> steps { get; set; }
        public Workflow() { }
    }
}
