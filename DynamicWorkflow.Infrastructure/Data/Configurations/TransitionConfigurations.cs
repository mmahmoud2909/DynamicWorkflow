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

            // 🔹 One Workflow => Many Transitions
            builder.HasOne(t => t.Workflow)
                .WithMany(w => w.Transitions)
                .HasForeignKey(t => t.WorkflowId)
                .OnDelete(DeleteBehavior.Restrict); 

            // 🔹 FromStep => OutgoingTransitions
            builder.HasOne(t => t.FromStep)
                .WithMany(s => s.OutgoingTransitions)
                .HasForeignKey(t => t.FromStepId)
                .OnDelete(DeleteBehavior.Restrict); 

            // 🔹 ToStep => IncomingTransitions
            builder.HasOne(t => t.ToStep)
                .WithMany(s => s.IncomingTransitions)
                .HasForeignKey(t => t.ToStepId)
                .OnDelete(DeleteBehavior.Restrict); 

            // 🔹 FromStatus relationship
            builder.HasOne(t => t.FromStatus)
                .WithMany() 
                .HasForeignKey(t => t.FromStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 ToStatus relationship
            builder.HasOne(t => t.ToStatus)
                .WithMany()
                .HasForeignKey(t => t.ToStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 ActionTypeEntity relationship
            builder.HasOne(t => t.ActionTypeEntity)
                .WithMany()
                .HasForeignKey(t => t.ActionTypeEntityId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
