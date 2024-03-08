namespace Contracts;

public interface ILoggerManager
{
    void LogTrace(string message, params object[] args);
    void LogDebug(string message, params object[] args);
    void LogInfo(string message, params object[] args);
    void LogWarn(string message, params object[] args);
    void LogError(string message, params object[] args);
    void LogFatal(string message, params object[] args);
}