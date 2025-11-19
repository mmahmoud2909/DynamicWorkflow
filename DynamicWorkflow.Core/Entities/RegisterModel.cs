using Microsoft.AspNetCore.Http;

namespace DynamicWorkflow.Core.Entities
{
    public class RegisterModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public IFormFile? ProfilePicUrl { get; set; }
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
