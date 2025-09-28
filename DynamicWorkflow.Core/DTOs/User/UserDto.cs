namespace DynamicWorkflow.Core.DTOs.User
{
    public class UserDto
    {
        private string v1;
        private string v2;
        private Guid? managerId;

        public UserDto()
        {
        }

        public UserDto(Guid id, string v1, string v2, string displayName, Guid departmentId, Guid? managerId, bool isPendingDeletion, DateTime registeredAt, string? profilePicUrl)
        {
            Id = id;
            this.v1 = v1;
            this.v2 = v2;
            DisplayName = displayName;
            DepartmentId = departmentId;
            this.managerId = managerId;
            IsPendingDeletion = isPendingDeletion;
            RegisteredAt = registeredAt;
            ProfilePicUrl = profilePicUrl;
        }

        public Guid Id { get; set; } 
        public string UserName { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string? ProfilePicUrl { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid ManagerId { get; set; }
        public DateTime RegisteredAt { get; set; }
        public string PasswordHash { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime? DeletionRequestedAt { get; set; }
        public bool IsPendingDeletion { get; set; }
    }
}
