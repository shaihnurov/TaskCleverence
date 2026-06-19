using System.Globalization;
using System.Text.RegularExpressions;
using TaskCleverence.Task3.Core.Interfaces;
using TaskCleverence.Task3.Core.Models;

namespace TaskCleverence.Task3.Core.Services.Parser;

/// <summary>
/// Парсер лог записей формата 1
/// Ожидаемый формат: <c>dd.mm.yyyy HH:mm:ss.fff level сообщение</c>
/// </summary>
public partial class Format1LogParser : ILogParser
{
    /// <summary>
    /// Регулярное выражение для разбора лог записи формата 1
    /// </summary>
    [GeneratedRegex(@"^(?<date>\d{2}\.\d{2}\.\d{4})\s+(?<time>\d{2}:\d{2}:\d{2}\.\d{3,7})\s+(?<level>[A-Z]+)\s+(?<message>.+)")]
    private static partial Regex LineRegex();

    /// <inheritdoc/>
    public bool TryParse(string line, out LogEntry? entry)
    {
        Match match = LineRegex().Match(line);

        if (match.Success)
        {
            try
            {
                string date = match.Groups["date"].Value;

                if (!LogLevelMapping.TryMap(match.Groups["level"].Value, out LogLevel level))
                {
                    entry = null;
                    return false;
                }

                entry = new LogEntry
                {
                    Date = DateOnly.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture),
                    Time = match.Groups["time"].Value,
                    Level = level,
                    Message = match.Groups["message"].Value,
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

        entry = null;
        return false;
    }
}
