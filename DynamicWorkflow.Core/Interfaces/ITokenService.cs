
namespace DynamicWorkflow.Core.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(string userId, string userName, string role);
    }

}
