using DynamicWorkflow.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWorkflow.Services.Services
{
    public class LoggingService<T>(ILogger<T> _logger) : ILoggingService<T>
    {
        public void LogError(string service, string message, Exception? ex = null)
        {
            _logger.LogError(ex, $"[{service}] --> {message}");
        }
        public void LogInfo(string service, string message)
        {
            _logger.LogInformation($"[{service}] --> {message}");
        }

        public void LogWarning(string service, string message)
        {
            _logger.LogWarning($"[{service}] --> {message}");
        }
    }
}
