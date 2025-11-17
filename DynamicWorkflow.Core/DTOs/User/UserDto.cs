namespace DynamicWorkflow.Core.DTOs.User
{
    public class UserDto
    {
        public UserDto() { }
        public Guid Id { get; set; } 
        public string UserName { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string? ProfilePicUrl { get; set; }
        public Guid DepartmentId { get; set; }
        public int? AppRoleId { get; set; }
        public Guid ManagerId { get; set; }
        public DateTime RegisteredAt { get; set; }
        public string PasswordHash { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime? DeletionRequestedAt { get; set; }
        public bool IsPendingDeletion { get; set; }

        public UserDto(Guid id, string userName, string email, string displayName,
                       Guid departmentId, int? appRoleId, Guid? managerId, bool isPendingDeletion,
                       DateTime registeredAt, string? profilePicUrl)
        {
            Id = id;
            UserName = userName;
            Email = email;
            DisplayName = displayName;
            DepartmentId = departmentId;
            AppRoleId = appRoleId;
            ManagerId = managerId ?? Guid.Empty;
            IsPendingDeletion = isPendingDeletion;
            RegisteredAt = registeredAt;
            ProfilePicUrl = profilePicUrl;
           
        }
    }
}
