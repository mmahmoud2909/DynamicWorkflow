using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWorkflow.Core.Entities
{
    public class WorkflowTransition:BaseEntity
    {
        public int FromStepId { get; set; }
        public int ToStepId { get; set; }
        public Workflow workflow { get; set; }
        public int WorkflowId { get; set; }

        public WorkflowStep? FromStep { get; set; }
        public WorkflowStep? ToStep { get; set; }

    }
}
