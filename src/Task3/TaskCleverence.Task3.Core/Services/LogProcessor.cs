using TaskCleverence.Task3.Core.Interfaces;
using TaskCleverence.Task3.Core.Models;

namespace TaskCleverence.Task3.Core.Services;

/// <summary>
/// Процесс обработки лог файлов, читает входной файл, параллельно парсит строки через все доступные парсеры
/// и записывает результат в выходные файлы
/// </summary>
public class LogProcessor(IEnumerable<ILogParser> logParser, ILogWriter logWriter) : ILogProcessor
{
    /// <inheritdoc/>
    public async Task ProcessAsync(string inputPath, string outputPath, string problemsPath)
    {
        var lines = await File.ReadAllLinesAsync(inputPath);

        var results = lines.AsParallel().AsOrdered().Select(line =>
        {
            if (string.IsNullOrWhiteSpace(line))
                return new ParseResult(false, null, line, true);

            foreach (var parser in logParser)
            {
                if (parser.TryParse(line, out LogEntry? entry))
                    return new ParseResult(true, entry, line, false);
            }

            return new ParseResult(false, null, line, false);
        }).Where(r => !r.IsEmpty).ToList();

        var parsedEntries = results.Where(r => r.IsSuccess).Select(r => r.Entry!).ToList();
        var problemLines = results.Where(r => !r.IsSuccess).Select(r => r.RawLine).ToList();

        if (parsedEntries.Count > 0)
            await logWriter.WriteAsync(parsedEntries, outputPath);

        if (problemLines.Count > 0)
            await logWriter.WriteInvalidAsync(problemLines, problemsPath);
    }

    /// <summary>
    /// Внутренняя запись результата парсинга одной строки
    /// </summary>
    /// <param name="IsSuccess">Успешно ли распарсена строка.</param>
    /// <param name="Entry">Распарсенная запись, <c>null</c> если парсинг не удался.</param>
    /// <param name="RawLine">Исходная строка из входного файла.</param>
    /// <param name="IsEmpty">Является ли строка пустой.</param>
    private record ParseResult(bool IsSuccess, LogEntry? Entry, string RawLine, bool IsEmpty);
}