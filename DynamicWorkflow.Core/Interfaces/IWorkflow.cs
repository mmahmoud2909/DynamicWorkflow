using DynamicWorkflow.Core.Entities;

namespace DynamicWorkflow.Core.Interfaces
{
    public interface IWorkflow
    {
        Task<WorkflowInstance?> GetByIdAsync(int id);
        Task AddAsync(WorkflowInstance instance);
        Task UpdateAsync(WorkflowInstance instance);
        Task SaveAsync(WorkflowInstance instance);
    }
}
