using DynamicWorkflow.Core.Enums;

namespace DynamicWorkflow.Core.DTOs.User
{
    public class CreateUserDto
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid? ManagerId { get; set; }
        public Roles Role {  get; set; }

    }
}
