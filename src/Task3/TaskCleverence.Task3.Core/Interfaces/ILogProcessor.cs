namespace TaskCleverence.Task3.Core.Interfaces;

/// <summary>
/// Интерфейс для общего процесса обработки лог-файлов
/// Координирует чтение входного файла, парсинг и запись результата
/// </summary>
public interface ILogProcessor
{
    /// <summary>
    /// Запускает обработку лог файла, читает входной файл, нормализует записи и сохраняет результат в выходные файлы
    /// </summary>
    /// <param name="inputPath">Путь к входному лог файлу</param>
    /// <param name="outputPath">Путь к выходному файлу</param>
    /// <param name="problemsPath">Путь к файлу для невалидных записей</param>
    Task ProcessAsync(string inputPath, string outputPath, string problemsPath);
}