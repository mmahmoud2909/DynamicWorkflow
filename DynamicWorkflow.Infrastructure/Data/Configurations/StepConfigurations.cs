using DynamicWorkflow.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DynamicWorkflow.Infrastructure.Data.Configurations
{
    public class StepConfigurations : IEntityTypeConfiguration<WorkflowStep>
    {
        public void Configure(EntityTypeBuilder<WorkflowStep> builder)
        {
            builder.ToTable("WorkflowSteps").HasKey(spk=>spk.Id);

            builder.Property(s=>s.Name).IsRequired().HasMaxLength(200);

            builder.Property(s => s.Comments).HasMaxLength(2000);

            builder.Property(s=>s.isEndStep).IsRequired();

            builder.HasOne(s => s.workflow)
                .WithMany(w => w.Steps)
                .HasForeignKey(s => s.WorkflowId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(st=>st.OutgoingTransitions)
                .WithOne(tr=>tr.FromStep)
                .HasForeignKey(sfk=>sfk.FromStepId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(st => st.IncomingTransitions)
                .WithOne(tr => tr.ToStep)
                .HasForeignKey(sfk => sfk.ToStepId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
