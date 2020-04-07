using System;

namespace AMAGE.Services
{
    public interface ILogService
    {
        string LogFileName { get; }

        void LogError(Exception ex);
        void LogMessage(string message);
    }
}
