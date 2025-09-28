using Microsoft.AspNetCore.Identity;

namespace DynamicWorkflow.Core.Entities.Users
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public ICollection<ApplicationUser> Users { get; set; } = new HashSet<ApplicationUser>();
        public string name { get; set; }
    }
}
