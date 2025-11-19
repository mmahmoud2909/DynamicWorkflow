using DynamicWorkflow.Core.Entities;

namespace DynamicWorkflow.Core.DTOs.User
{
    public class CreateUserDto
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public String Role { get; set; } = "Employee";
        public string Email { get; set; }
        public string Password { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid? ManagerId { get; set; }
    }
}
