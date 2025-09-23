using Microsoft.AspNetCore.Identity;

namespace DynamicWorkflow.Core.Entities.Users
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? ProfilePicUrl { get; set; }
        public DateTime? DeletionRequestedAt { get; set; }
        public bool IsPendingDeletion { get; set; }
        public string DisplayName { get; set; }
        public DateTime RegisteredAt { get; set; }
        public int DepartmentId { get; set; }
        public Guid? ManagerId { get; set; }
        public Guid RoleId { get; set; }

    }
}
