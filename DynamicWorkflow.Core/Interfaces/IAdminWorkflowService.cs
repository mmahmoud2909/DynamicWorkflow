using DynamicWorkflow.Core.DTOs.StepDto;
using DynamicWorkflow.Core.DTOs.Transition;
using DynamicWorkflow.Core.DTOs.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWorkflow.Core.Interfaces
{
    public interface IAdminWorkflowService
    {
        // Workflows
        Task<List<WorkflowDto>> GetAllWorkflowsAsync();
        Task<WorkflowDto?> GetWorkflowByIdAsync(int id);
        Task<int> CreateWorkflowAsync(CreateWorkflowDto dto);
        Task UpdateWorkflowAsync(int id, CreateWorkflowDto dto);
        Task DeleteWorkflowAsync(int id);

        // Steps
        Task<int> AddStepAsync(int workflowId, CreateStepDto dto);
        Task UpdateStepAsync(int stepId, UpdateStepDto dto);
        Task DeleteStepAsync(int stepId);

        // Transitions (create/delete/list)
        Task<int> AddTransitionAsync(int workflowId, CreateTransitionDto dto);
        Task DeleteTransitionAsync(int transitionId);
    }
}
