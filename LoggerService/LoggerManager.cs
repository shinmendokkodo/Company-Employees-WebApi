using Contracts;
using NLog;

namespace LoggerService;

public class LoggerManager : ILoggerManager
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public LoggerManager() { }

    public void LogTrace(string message, params object[] args) => Logger.Trace(message, args);

    public void LogDebug(string message, params object[] args) => Logger.Debug(message, args);

    public void LogError(string message, params object[] args) => Logger.Error(message, args);

    public void LogFatal(string message, params object[] args) => Logger.Fatal(message, args);

    public void LogInfo(string message, params object[] args) => Logger.Info(message, args);

    public void LogWarn(string message, params object[] args) => Logger.Warn(message, args);
}