using DynamicWorkflow.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWorkflow.Core.Interfaces
{
    public interface IAccountService
    {
        //Task<(bool isSuccess, string message, string? token)> RegisterUserAsync(RegisterModel model);
        Task<(bool isSuccess, string token, string message, bool isDeletionCancelled)> LoginUserAsync(LoginModel model);
    }
}
