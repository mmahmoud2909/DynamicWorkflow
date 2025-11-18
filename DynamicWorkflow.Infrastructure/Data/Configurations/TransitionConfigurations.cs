using DynamicWorkflow.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DynamicWorkflow.Infrastructure.Data.Configurations
{
    public class TransitionConfigurations : IEntityTypeConfiguration<WorkflowTransition>
    {
        public void Configure(EntityTypeBuilder<WorkflowTransition> builder)
        {
            builder.ToTable("WorkflowTransitions");
            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.Workflow)
                .WithMany(w => w.Transitions)
                .HasForeignKey(t => t.WorkflowId)
                .OnDelete(DeleteBehavior.Restrict); 
            
            builder.HasOne(t => t.FromStep)
                .WithMany(s => s.OutgoingTransitions)
                .HasForeignKey(t => t.FromStepId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.HasOne(t => t.ToStep)
                .WithMany(s => s.IncomingTransitions)
                .HasForeignKey(t => t.ToStepId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.HasOne(t => t.FromStatus)
                .WithMany(t=>t.WorkflowTransitions) 
                .HasForeignKey(t => t.FromStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.ToStatus)
                .WithMany()
                .HasForeignKey(t => t.ToStatusId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(wt => wt.ActionTypeEntityId)
                   .IsRequired();

            builder.Property(wt => wt.PerformedBy)
                   .HasMaxLength(450);

            builder.Property(wt => wt.CreatedBy)
                   .HasMaxLength(450)
                   .IsRequired();
            builder.HasOne(t => t.ActionTypeEntity)
                .WithMany()
                .HasForeignKey(t => t.ActionTypeEntityId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
