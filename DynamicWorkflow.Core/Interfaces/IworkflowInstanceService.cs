using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWorkflow.Core.Interfaces
{
    public interface IworkflowInstanceService
    {
        // <summary>
        /// Create a new workflow instance for a given workflow and user
        /// </summary>
        /// <param name="workflowId">The ID of the workflow template</param>
        /// <param name="userId">The ID of the user who creates the instance</param>
        /// <returns>The created WorkflowInstance</returns>
        Task<WorkflowInstance> CreateInstanceAsync(int workflowId, Guid userId);

        /// <summary>
        /// Get all workflow instances for a specific user
        /// </summary>
        Task<IList<WorkflowInstance>> GetInstancesForUserAsync(Guid userId);

        /// <summary>
        /// Move the workflow instance to the next step based on transitions
        /// </summary>
        Task<WorkflowInstance> MoveToNextStepAsync(int instanceId, Guid userId, ActionType action, string? comments = null);

        /// <summary>
        /// Get details of a workflow instance
        /// </summary>
        Task<WorkflowInstance?> GetInstanceByIdAsync(int instanceId);





    }
}
