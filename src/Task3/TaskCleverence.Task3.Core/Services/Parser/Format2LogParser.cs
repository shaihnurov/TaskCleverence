using System.Globalization;
using TaskCleverence.Task3.Core.Interfaces;
using TaskCleverence.Task3.Core.Models;

namespace TaskCleverence.Task3.Core.Services.Parser;

/// <summary>
/// Парсер лог-записей формата 2
/// Ожидаемый формат: <c>yyyy-mm-dd hh:mm:ss.ffff| LEVEL|threadId|CallingMethod| Сообщение</c>
/// </summary>
public class Format2LogParser : ILogParser
{
    /// <inheritdoc/>
    public bool TryParse(string line, out LogEntry? entry)
    {
        var parts = line.Split('|');

        if (parts.Length < 5)
        {
            entry = null;
            return false;
        }

        try
        {
            var dateTime = parts[0].Split(' ');
            var date = dateTime[0];
            var time = dateTime[1];

            if (!LogLevelMapping.TryMap(parts[1].Trim(), out LogLevel level))
            {
                entry = null;
                return false;
            }

            entry = new LogEntry
            {
                Date = DateOnly.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                Time = time,
                Level = level,
                CallingMethod = string.IsNullOrWhiteSpace(parts[3]) ? "DEFAULT" : parts[3],
                Message = string.Join("|", parts[4..]),
            };

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to parse line: {ex.Message}");
            entry = null;
            return false;
        }
    }
}
