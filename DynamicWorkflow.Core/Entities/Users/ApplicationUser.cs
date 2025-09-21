using Microsoft.AspNetCore.Identity;

namespace DynamicWorkflow.Core.Entities.Users
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ProfilePicUrl { get; set; }
        public DateTime? DeletionRequestedAt { get; set; }
        public bool IsPendingDeletion { get; set; }
        public string DisplayName { get; set; }
        public DateTime RegisteredAt { get; set; }
    }
}
