using TaskCleverence.Task3.Core.Models;

namespace TaskCleverence.Task3.Core.Interfaces;

/// <summary>
/// Интерфейс для записи лог записей в выходные файлы
/// </summary>
public interface ILogWriter
{
    /// <summary>
    /// Записывает лог запись в выходной файл
    /// </summary>
    /// <param name="entries">Коллекция лог записей, для батчевой записи</param>
    /// <param name="outputPath">Путь к выходному файлу</param>
    Task WriteAsync(IEnumerable<LogEntry> entries, string outputPath);

    /// <summary>
    /// Записывает невалидную строку в файл проблемных записей
    /// </summary>
    /// <param name="rawLines">Коллекция исходных строк, для батчевой записи</param>
    /// <param name="problemsPath">Путь к файлу проблемных записей</param>
    Task WriteInvalidAsync(IEnumerable<string> rawLines, string problemsPath);
}