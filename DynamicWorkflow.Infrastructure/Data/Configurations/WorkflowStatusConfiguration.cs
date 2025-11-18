using DynamicWorkflow.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

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
        }
    }
}