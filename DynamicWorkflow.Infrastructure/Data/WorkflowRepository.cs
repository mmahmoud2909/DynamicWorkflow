using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Enums;
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

        //public static Workflow GetWorkflow()
        //{
        //    if (_workflow != null) return _workflow;

        //    _workflow = new Workflow
        //    {
        //        Id = 1,
        //        Name = "Vacation Request Workflow",
        //        Description = "Vacation requested due to Gradution Party for class 2025",
        //        Steps = new List<WorkflowStep>
        //        {
        //        new WorkflowStep { Id = 1, Name = "Vacation Request", stepActionTypes = ActionType.Create, stepStatus = Status.InProgress },
        //        new WorkflowStep { Id = 2, Name = "N+1 Approval", stepActionTypes = ActionType.Hold, stepStatus = Status.ONHold  },
        //        new WorkflowStep { Id = 3, Name = "Manager Approval", stepActionTypes = ActionType.Skip, stepStatus = Status.Skipped  },
        //        new WorkflowStep { Id = 4, Name = "HR Validation", stepActionTypes = ActionType.Reject, stepStatus = Status.Rejected}
        //        }
        //    };

        //    return _workflow;
        //}
    }
}
