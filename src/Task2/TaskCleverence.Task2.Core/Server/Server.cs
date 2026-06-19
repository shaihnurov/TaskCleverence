namespace TaskCleverence.Task2.Core.Server;

/// <summary>
/// Статический класс с потокобезопасным счётчиком
/// </summary>
public static class Server
{
    /// <summary>
    /// Счётчик, доступный для чтения и записи через методы
    /// </summary>
    private static int _count;

    /// <summary>
    /// Обеспечивает потокобезопасное параллельное чтение и одну запись
    /// </summary>
    private static readonly ReaderWriterLockSlim _lock = new();

    /// <summary>
    /// Возвращает текущее значение счётчика
    /// </summary>
    /// <returns>Текущее значение <see cref="_count"/></returns>
    public static int GetCount()
    {
        _lock.EnterReadLock();

        try
        {
            return _count;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <summary>
    /// Добавляет указанное значение к счётчику
    /// </summary>
    /// <param name="value">Значение для добавления к счётчику</param>
    public static void AddToCount(int value)
    {
        _lock.EnterWriteLock();

        try
        {
            checked
            {
                _count += value;
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Сбрасывает счётчик в ноль
    /// Решил добавить его для тестов
    /// </summary>
    public static void Reset()
    {
        _lock.EnterWriteLock();

        try
        {
            _count = 0;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Освобождает ресурсы блокировки
    /// </summary>
    public static void Dispose()
    {
        _lock.Dispose();
    }
}