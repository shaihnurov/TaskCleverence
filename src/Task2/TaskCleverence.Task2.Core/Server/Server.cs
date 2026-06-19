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
    private readonly static ReaderWriterLockSlim _readerWriterLockSlim = new();

    /// <summary>
    /// Возвращает текущее значение счётчика
    /// </summary>
    /// <returns>Текущее значение <see cref="_count"/></returns>
    public static int GetCount()
    {
        _readerWriterLockSlim.EnterReadLock();

        try
        {
            return _count;
        }
        finally
        {
            _readerWriterLockSlim.ExitReadLock();
        }
    }

    /// <summary>
    /// Добавляет указанное значение к счётчику
    /// </summary>
    /// <param name="value">Значение для добавления к счётчику</param>
    public static void AddToCount(int value)
    {
        _readerWriterLockSlim.EnterWriteLock();

        try
        {
            // тут можно использовать checked чтобы получить исключение при переполнении int
            checked
            {
                _count += value;
            }
        }
        finally
        {
            _readerWriterLockSlim.ExitWriteLock();
        }
    }

    /// <summary>
    /// Сбрасывает счётчик в ноль
    /// Решил добавить его для тестов
    /// </summary>
    public static void Reset()
    {
        _readerWriterLockSlim.EnterWriteLock();

        try
        {
            _count = 0;
        }
        finally
        {
            _readerWriterLockSlim.ExitWriteLock();
        }
    }
}