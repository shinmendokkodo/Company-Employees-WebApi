namespace Contracts;

public interface ILoggerManager
{
    void LogInfo(string message);
    void LogInfo<T>(T input);
    void LogWarn(string message);
    void LogWarn<T>(T input);
    void LogDebug(string message);
    void LogDebug<T>(T input);
    void LogError(string message);
    void LogError<T>(T input);
}