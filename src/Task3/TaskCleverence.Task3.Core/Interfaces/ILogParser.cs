using TaskCleverence.Task3.Core.Models;

namespace TaskCleverence.Task3.Core.Interfaces;

/// <summary>
/// Интерфейс для парсера лог записей. Каждая реализация отвечает за распознавание и разбор одного конкретного формата
/// </summary>
public interface ILogParser
{
    /// <summary>
    /// Пытается распарсить строку лог файла в <see cref="LogEntry"/>
    /// </summary>
    /// <param name="line">Строка из входного лог файла</param>
    /// <param name="entry">Распарсенная запись, либо null если строка не соответствует формату</param>
    /// <returns>true если строка успешно распарсена, иначе false</returns>
    bool TryParse(string line, out LogEntry? entry);
}