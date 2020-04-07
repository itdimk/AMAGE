using System;
using System.IO;

namespace AMAGE.Services
{
    public sealed class LogService : ILogService
    {
        public string LogFileName { get; private set; } = "log.txt";

        public void LogError(Exception exception)
        {
            if (exception != null)
            {
                string textToWrite = $"\r\n\r\n {DateTime.Now}\r\n {exception.Message}\r\n\r\n {exception.StackTrace}";
                File.AppendAllText(LogFileName, textToWrite);
            }
            else
                LogMessage("Unknown error");
        }

        public void LogMessage(string message)
        {
            string textToWrite = $"\r\n\r\n {DateTime.Now}\r\n {message}";
            File.AppendAllText(LogFileName, textToWrite);
        }
    }
}
