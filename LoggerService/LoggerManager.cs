using System.Text.Json;
using Contracts;
using NLog;

namespace LoggerService;

public class LoggerManager : ILoggerManager
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public LoggerManager() { }

    public void LogDebug(string message) => Logger.Debug(message);

    public void LogDebug<T>(T input)
    {
        string json = JsonSerialize(input);
        Logger.Debug(json);
    }

    public void LogError(string message) => Logger.Error(message);

    public void LogError<T>(T input)
    {
        string json = JsonSerialize(input);
        Logger.Error(json);
    }

    public void LogInfo(string message) => Logger.Info(message);

    public void LogInfo<T>(T input)
    {
        string json = JsonSerialize(input);
        Logger.Info(json);
    }

    public void LogWarn(string message) => Logger.Warn(message);

    public void LogWarn<T>(T input)
    {
        string json = JsonSerialize(input);
        Logger.Warn(json);
    }

    private static string JsonSerialize<T>(T input)
    {
        return JsonSerializer.Serialize(input);
    }
}
