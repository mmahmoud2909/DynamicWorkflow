using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWorkflow.Core.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(string email, string password, string displayName);
        Task<string> LoginAsync(string email, string password);
    }
}
