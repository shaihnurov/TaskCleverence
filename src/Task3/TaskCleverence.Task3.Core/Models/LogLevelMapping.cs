namespace TaskCleverence.Task3.Core.Models;

public static class LogLevelMapping
{
    public static bool TryMap(string value, out LogLevel level)
    {
        switch (value)
        {
            case "INFORMATION":
            case "INFO":
                level = LogLevel.INFO;
                return true;
            case "WARNING":
            case "WARN":
                level = LogLevel.WARN;
                return true;
            case "ERROR":
                level = LogLevel.ERROR;
                return true;
            case "DEBUG":
                level = LogLevel.DEBUG;
                return true;
            default:
                level = default;
                return false;
        }
    }
}