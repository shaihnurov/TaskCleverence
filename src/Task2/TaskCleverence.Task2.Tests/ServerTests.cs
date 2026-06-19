using TaskCleverence.Task2.Core.Server;

namespace TaskCleverence.Task2.Tests;

/// <summary>
/// Тесты для <see cref="Server"/> проверяют корректность счётчика при одиночном и параллельном доступе
/// </summary>
public class ServerTests
{
    /// <summary>
    /// После вызова Reset счётчик должен вернуться в ноль
    /// </summary>
    [Fact]
    public void Reset_AfterAddToCount_ReturnsZero()
    {
        Server.AddToCount(100);

        Server.Reset();

        Assert.Equal(0, Server.GetCount());
    }

    #region GetCount
    /// <summary>
    /// Начальное значение счётчика после сброса должно быть равно нулю
    /// </summary>
    [Fact]
    public void GetCount_InitialValue_ReturnsZero()
    {
        Server.Reset();

        int result = Server.GetCount();

        Assert.Equal(0, result);
    }

    /// <summary>
    /// Несколько потоков могут читать счётчик параллельно все должны получить корректное значение
    /// </summary>
    [Fact]
    public async Task GetCount_ParallelReaders_AllReturnCorrectValue()
    {
        Server.Reset();
        var tasks = new Task<int>[10];

        for (int i = 0; i < tasks.Length; i++)
        {
            tasks[i] = Task.Run(() => Server.GetCount());
        }

        await Task.WhenAll(tasks);

        bool allCorrect = tasks.All(t => t.Result == 0);
        Assert.True(allCorrect);
    }
    #endregion

    #region AddToCount
    /// <summary>
    /// Добавление положительного значения корректно увеличивает счётчик
    /// </summary>
    [Fact]
    public void AddToCount_AddsValue_Correctly()
    {
        Server.Reset();

        Server.AddToCount(1);

        Assert.Equal(1, Server.GetCount());
    }

    /// <summary>
    /// Добавление отрицательного значения корректно уменьшает счётчик
    /// </summary>
    [Fact]
    public void AddToCount_AddsNegativeValue_Correctly()
    {
        Server.Reset();

        Server.AddToCount(-1);

        Assert.Equal(-1, Server.GetCount());
    }

    /// <summary>
    /// При переполнении int должен выбросить OverflowException
    /// Заполняем счётчик до MaxValue а затем добавляем ещё 1
    /// </summary>
    [Fact]
    public void AddToCount_Overflow_ThrowsOverflowException()
    {
        Server.Reset();
        Server.AddToCount(int.MaxValue);

        Assert.Throws<OverflowException>(() => Server.AddToCount(1));
    }

    /// <summary>
    /// Несколько потоков пишут в счётчик параллельно итоговое значение должно точно соответствовать сумме всех операций
    /// </summary>
    [Fact]
    public async Task AddToCount_ParallelWriters_CountIsConsistent()
    {
        Server.Reset();
        var tasks = new Task[100];

        for (int i = 0; i < tasks.Length; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 1000; j++)
                    Server.AddToCount(1);
            });
        }

        await Task.WhenAll(tasks);

        Assert.Equal(100000, Server.GetCount());
    }
    #endregion
}