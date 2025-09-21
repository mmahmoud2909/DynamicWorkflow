using Microsoft.AspNetCore.Identity;

namespace DynamicWorkflow.Core.Entities.Users
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string DisplayName { get; set; }
        public DateTime RegisteredAt { get; set; }
    }
}
