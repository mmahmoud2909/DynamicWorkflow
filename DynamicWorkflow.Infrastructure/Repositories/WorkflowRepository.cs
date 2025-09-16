using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWorkflow.Infrastructure.Repositories
{
    public class WorkflowRepository
    {
        private static Workflow? _workflow;

        public static Workflow GetWorkflow()
        {
            if (_workflow != null) return _workflow;

            _workflow = new Workflow
            {
                Id = 1,
                name = "Vacation Request Workflow",
                description = "Vacation requested due to Gradution Party for class 2025",
                steps = new List<WorkflowStep>
                {
                new WorkflowStep { Id = 1, stepName = "Vacation Request", stepActionTypes = ActionType.Create, stepStatus = Status.InProgress },
                new WorkflowStep { Id = 2, stepName = "N+1 Approval", stepActionTypes = ActionType.Hold, stepStatus = Status.ONHold  },
                new WorkflowStep { Id = 3, stepName = "Manager Approval", stepActionTypes = ActionType.Skip, stepStatus = Status.Skipped  },
                new WorkflowStep { Id = 4, stepName = "HR Validation", stepActionTypes = ActionType.Reject, stepStatus = Status.Rejected}
                }
            };

            return _workflow;
        }
    }
}

