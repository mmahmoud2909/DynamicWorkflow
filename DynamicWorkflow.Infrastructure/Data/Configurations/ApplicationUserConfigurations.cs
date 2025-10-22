using DynamicWorkflow.Core.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DynamicWorkflow.Infrastructure.Data.Configurations
{
    public class ApplicationUserConfigurations : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasOne(u=>u.Department).WithMany(d=>d.Users).HasForeignKey(fk=>fk.DepartmentId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}
