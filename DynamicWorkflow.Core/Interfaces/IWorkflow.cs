using DynamicWorkflow.Core.DTOs.StepDto;
using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Entities.Users;

namespace DynamicWorkflow.Core.Interfaces
{
    public interface IWorkflow
    {
        Task<WorkflowInstance?> GetByIdAsync(int id);
        Task SaveAsync(WorkflowInstance instance);
        Task AddAsync(WorkflowInstance instance);
        Task UpdateAsync(WorkflowInstance instance);

        Task MakeActionAsync(Workflow workflow, int stepId, int actionTypeEntityId, ApplicationUser currentUser);
    }
}
