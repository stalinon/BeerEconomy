using Microsoft.Extensions.Logging;

namespace BeerEconomy.Common.Helpers.Logging;

/// <summary>
///     Статический логгер
/// </summary>
public static class StaticLogger
{
    private static ILoggerFactory _loggerFactory = null!;

    /// <summary>
    ///     Конфигурация статического логгера
    /// </summary>
    public static void Configure(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    /// <summary>
    ///     Создать логгер
    /// </summary>
    public static ILogger<T> CreateLogger<T>()
    {
        return _loggerFactory.CreateLogger<T>();
    }

    /// <summary>
    ///     Создать логгер
    /// </summary>
    public static ILogger CreateLogger(string categoryName)
    {
        return _loggerFactory.CreateLogger(categoryName);
    }
}