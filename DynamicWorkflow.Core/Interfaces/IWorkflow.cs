using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWorkflow.Core.Interfaces
{
    public interface IWorkflow
    {
        public Task MakeAction (Workflow workflow, int stepId, ActionType action);
    }
}
