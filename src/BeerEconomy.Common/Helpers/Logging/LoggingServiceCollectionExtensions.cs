using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BeerEconomy.Common.Helpers.Logging;

/// <summary>
///     Расширение <see cref="IServiceCollection"/> для конфигурации логгера
/// </summary>
public static class LoggingServiceCollectionExtensions
{
    /// <summary>
    ///     Конфигурация логгера
    /// </summary>
    public static IServiceCollection AddCustomLogging(this IServiceCollection services)
    {
        services.AddLogging(configure =>
        {
            configure.ClearProviders();
            configure.AddConsole();
            configure.AddDebug();
            configure.AddEventSourceLogger();
        });

        return services;
    }
    
    /// <summary>
    ///     Конфигурация логгера
    /// </summary>
    public static IApplicationBuilder UseCustomLogging(this IApplicationBuilder services)
    {
        StaticLogger.Configure(services.ApplicationServices.GetRequiredService<ILoggerFactory>());

        return services;
    }
}