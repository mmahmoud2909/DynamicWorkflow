namespace DynamicWorkflow.Core.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(Guid userId, string userName, string role);
    }
}
