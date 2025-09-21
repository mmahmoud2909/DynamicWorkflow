using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWorkflow.Core.Entities
{
    public class WorkflowTransition:BaseEntity
    {
        public Guid FromStepId { get; set; }
        public Guid ToStepId { get; set; }

        public WorkflowStep? FromStep { get; set; }
        public WorkflowStep? ToStep { get; set; }

    }
}
