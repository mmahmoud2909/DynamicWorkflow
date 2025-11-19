using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Interfaces.Repositories;
using DynamicWorkflow.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Security.Claims;

namespace DynamicWorkflow.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationIdentityDbContext _dbContext;
        private Hashtable _repositories;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public UnitOfWork(ApplicationIdentityDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _repositories = new Hashtable();
            _httpContextAccessor = httpContextAccessor;
        }
        
        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var key = typeof(TEntity).Name;
            if (!_repositories.ContainsKey(key))
            {
                var repository = new GenericRepository<TEntity>(_dbContext);
                _repositories.Add(key, repository);
            }
            return _repositories[key] as IGenericRepository<TEntity>;
        }
        
        public async Task<int> CompleteAsync()
        {
            UpdateAuditFields();
            return await _dbContext.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
            => await _dbContext.DisposeAsync();

        protected string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? "System";
        }

        private void UpdateAuditFields()
        {
            var entries = _dbContext.ChangeTracker.Entries<BaseEntity>();
            var currentUserId = GetCurrentUserId();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    if (string.IsNullOrEmpty(entry.Entity.CreatedBy))
                    {
                        entry.Entity.CreatedBy = currentUserId;
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = currentUserId;
                }
            }
        }
    }
}
