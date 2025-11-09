using DynamicWorkflow.Core.Entities;

namespace DynamicWorkflow.Core.Interfaces
{
    public interface IAccountService
    {
        Task<(bool isSuccess, string token, string message, bool isDeletionCancelled)> LoginUserAsync(LoginModel model);
    }
}
