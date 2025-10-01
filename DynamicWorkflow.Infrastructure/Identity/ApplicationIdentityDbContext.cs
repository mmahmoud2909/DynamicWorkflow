using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Entities.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DynamicWorkflow.Infrastructure.Identity
{
    public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options) : base(options)
        {
            //_options = options;
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
        }
        public DbSet<ApplicationUser>ApplicationUsers { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<Workflow> Workflows {  get; set; }
        public DbSet<WorkflowStep> WorkflowSteps { get; set; }
        public DbSet<WorkflowInstance> WorkflowInstances { get; set; }
        public DbSet<WorkFlowInstanceStep> WorkFlowInstanceSteps { get; set; }
        public DbSet<WorkflowTransition>WorkflowTransitions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<StepRole> StepRoles { get; set; }

    }
}
