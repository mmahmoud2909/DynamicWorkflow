using DynamicWorkflow.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DynamicWorkflow.Infrastructure.Data.Configurations
{
    public class ActionTypeEntityConfiguration : IEntityTypeConfiguration<ActionTypeEntity>
    {
        public void Configure(EntityTypeBuilder<ActionTypeEntity> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd(); // auto identity
            builder.Property(a => a.Name)
                   .IsRequired()
                   .HasMaxLength(100);
        }
    }
}

