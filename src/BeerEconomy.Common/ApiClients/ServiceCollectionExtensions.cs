using BeerEconomy.Common.ApiClients.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace BeerEconomy.Common.ApiClients;

/// <summary>
///     Расширения <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Добавить сервисы АПИ
    /// </summary>
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddScoped<IPriceService, ApiPriceService>();
        services.AddScoped<ISourceService, ApiSourceService>();
        services.AddScoped<IBeerService, ApiBeerService>();
        services.AddScoped<ICollectorService, ApiCollectorService>();

        return services;
    }
}