using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Enums;
using DynamicWorkflow.Core.Interfaces;
using DynamicWorkflow.Infrastructure.Identity;
using DynamicWorkflow.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWorkflow.Infrastructure.Data
{
    public class WorkflowRepository : IWorkflow
    {
        private readonly ApplicationIdentityDbContext _db;

        public WorkflowRepository(ApplicationIdentityDbContext db) => _db = db;

        public Task<WorkflowInstance?> GetByIdAsync(Guid id) =>
            _db.WorkflowInstances.FindAsync(id).AsTask();

        public Task MakeAction(Workflow workflow, int stepId, ActionType action)
        {
            throw new NotImplementedException();
        }

        public async Task SaveAsync(WorkflowInstance instance)
        {
            _db.Update(instance);
            await _db.SaveChangesAsync();
        }

        public async Task AddAsync(WorkflowInstance instance)
        {
            _db.Workflows.Add(instance);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(WorkflowInstance instance)
        {
            _db.Workflows.Update(instance);
            await _db.SaveChangesAsync();
        }
    }

}
