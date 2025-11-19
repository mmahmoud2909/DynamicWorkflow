using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Entities.Users;
using DynamicWorkflow.Core.Interfaces;
using DynamicWorkflow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace DynamicWorkflow.Infrastructure.Data
{
    public class WorkflowRepository : IWorkflow
    {
        private readonly ApplicationIdentityDbContext _db;
        private static Workflow? _workflow;

        public WorkflowRepository(ApplicationIdentityDbContext db) => _db = db;

        public async Task<WorkflowInstance?> GetByIdAsync(int id)
        {
            return await _db.WorkflowInstances
                .Include(w => w.Workflow)
                .Include(w => w.CurrentStep)
                .FirstOrDefaultAsync(w => w.Id == id);
        }
        public async Task AddAsync(WorkflowInstance instance)
        {
            await _db.WorkflowInstances.AddAsync(instance);
            await _db.SaveChangesAsync();
        }
        public async Task UpdateAsync(WorkflowInstance instance)
        {
            _db.WorkflowInstances.Update(instance);
            await _db.SaveChangesAsync();
        }

        public async Task SaveAsync(WorkflowInstance instance)
        {
            _db.Entry(instance).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public Task MakeActionAsync(Workflow workflow, int stepId, int actionTypeEntityId, ApplicationUser currentUser)
        {
            throw new NotImplementedException();
        }

        public Task<List<WorkflowStep>> GetAllStepsAsync(int workflowId)
        {
            throw new NotImplementedException();
        }

        public Task<WorkflowStep?> GetStepByIdAsync(int stepId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CanUserPerformStepAsync(int stepId, ApplicationUser user)
        {
            throw new NotImplementedException();
        }
    }
}
