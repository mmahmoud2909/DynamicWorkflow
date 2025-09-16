using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Enums;
using DynamicWorkflow.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWorkflow.Services.Services
{
    public class StepService : IWorkflow
    {
        public Task MakeAction(Workflow workflow, int stepId, ActionType action)
        {
            var step = workflow.steps.FirstOrDefault(s => s.Id == stepId);
            if (step == null)
            {
                throw new Exception("Step Does Not Exist");
            }
            else
            {
                switch (action)
                {
                    case ActionType.Accept:
                        step.stepStatus = Status.Accepted;
                        break;
                    case ActionType.Skip:
                        step.stepStatus = Status.Skipped;
                        break;
                    case ActionType.Reject:
                        step.stepStatus = Status.Rejected;
                        break;
                    case ActionType.Hold:
                        step.stepStatus = Status.ONHold;
                        break;
                    case ActionType.Notify:
                        step.stepStatus = Status.Completed;
                        break;
                    default:
                        step.stepStatus = Status.InProgress;
                        break;
                }
                return Task.CompletedTask;
            }
        }

        public Task GetAllSteps(Workflow workflow)
        {
            var result = workflow.steps;
            return (Task)result;
        }

    }
}
