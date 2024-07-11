using System.Reflection;
using BeerEconomy.Common.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace BeerEconomy.Common.Helpers;

/// <summary>
///     Конфигурация <c>Swagger</c>
/// </summary>
public static class SwaggerConfiguration
{
    /// <inheritdoc cref="SwaggerConfiguration"/>
    public static void UseSwaggerConfig(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        });
    }

    /// <inheritdoc cref="SwaggerConfiguration"/>
    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SchemaFilter<SwaggerExcludeFilter>();
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1"
            });
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }
}