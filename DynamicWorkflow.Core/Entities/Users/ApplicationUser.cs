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
        public Guid DepartmentId { get; set; }
        public Guid? ManagerId { get; set; }
        public ICollection<ApplicationRole> Roles { get; set; } = new HashSet<ApplicationRole>();
        public AppRole? AppRole { get; set; }
        public Department Department { get; set; }
        public int? AppRoleId { get; set; }
    }
}
