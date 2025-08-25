using Microsoft.Extensions.Logging;

namespace Indium.Common.Modules
{
    public interface ILogService
    {
        public void WriteLog(string message);
    }
    public class LogService : ILogService
    {
        public ILogger Logger { get; set; }
        public void WriteLog(string message)
        {
            Logger.LogInformation(message);
        }
    }
}
