namespace TaskCleverence.Task3.Core.Models;

/// <summary>
/// Модель лог записи, не зависящая от входного формата
/// </summary>
public class LogEntry
{
    /// <summary>
    /// Дата записи
    /// </summary>
    public DateOnly Date { get; init; }

    /// <summary>
    /// Время записи в исходном формате
    /// </summary>
    public string Time { get; init; } = string.Empty;

    /// <summary>
    /// Нормализованный уровень логирования
    /// </summary>
    public LogLevel Level { get; init; }

    /// <summary>
    /// Вызвавший метод, если отсутствует во входной записи подставляется DEFAULT
    /// </summary>
    public string CallingMethod { get; init; } = "DEFAULT";

    /// <summary>
    /// Сообщение
    /// </summary>
    public string Message { get; init; } = string.Empty;
}