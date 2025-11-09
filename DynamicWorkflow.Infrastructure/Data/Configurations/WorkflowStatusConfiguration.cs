using DynamicWorkflow.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DynamicWorkflow.Infrastructure.Data.Configurations
{
    public class WorkflowStatusConfiguration : IEntityTypeConfiguration<WorkflowStatus>
    {
        public void Configure(EntityTypeBuilder<WorkflowStatus> builder)
        {
            builder.ToTable("WorkflowStatuses");

            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id);
            builder.Property(s => s.Name)
                   .IsRequired()
                   .HasMaxLength(100);
            //builder.HasMany(wfa=>wfa.WorkflowInstanceActions).WithOne(e=>e.workflowStatus).HasForeignKey(e=>e.WorkflowStatusId).OnDelete(DeleteBehavior.NoAction);

        }
    }
}