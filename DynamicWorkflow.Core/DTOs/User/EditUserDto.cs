using Microsoft.AspNetCore.Http;

namespace DynamicWorkflow.Core.DTOs.User
{
    public class EditUserDto
    {
        public IFormFile? ProfilePicUrl { get; set; }
        public string? Username { get; set; }
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmNewPassword { get; set; }
        public string? Email { get; set; }
        public int? AppRoleId { get; set; }
    }
}
