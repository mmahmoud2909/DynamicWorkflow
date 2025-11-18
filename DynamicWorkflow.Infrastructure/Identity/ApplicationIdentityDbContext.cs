using DynamicWorkflow.Core.Entities;
using DynamicWorkflow.Core.Entities.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicWorkflow.Infrastructure.Identity
{
    public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);

           builder.Entity<Workflow>()
                    .HasOne(w => w.WorkflowStatus)
                    .WithMany(s => s.workflows)
                    .HasForeignKey(w => w.WorkflowStatusId)
                    .OnDelete(DeleteBehavior.Restrict);

        }
        public DbSet<ApplicationUser>ApplicationUsers { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<Workflow> Workflows {  get; set; }
        public DbSet<WorkflowStep> WorkflowSteps { get; set; }
        public DbSet<WorkflowInstance> WorkflowInstances { get; set; }
        public DbSet<WorkFlowInstanceStep> WorkFlowInstanceSteps { get; set; }
        public DbSet<WorkflowTransition>WorkflowTransitions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<ActionTypeEntity> ActionTypes { get; set; }
        public DbSet<WorkflowStatus> WorkflowStatuses { get; set; }
    }
}
