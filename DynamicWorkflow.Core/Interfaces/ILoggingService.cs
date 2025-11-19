namespace DynamicWorkflow.Core.Interfaces
{
    public interface ILoggingService<T>
    {
        void LogInfo(string service, string message);
        void LogWarning(string service, string message);
        void LogError(string service, string message, Exception? ex = null);
    }
}
