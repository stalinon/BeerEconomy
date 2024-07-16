using BeerEconomy.Common.Helpers.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BeerEconomy.Common.Helpers;

/// <summary>
///     Мидлварь для защити по ключу
/// </summary>
public sealed class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;

    /// <inheritdoc cref="ApiKeyMiddleware" />
    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <inheritdoc cref="ApiKeyMiddleware" />
    public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
    {
        if (!context.Request.Headers.TryGetValue(Configs.API_KEY_HEADER_NAME, out var extractedApiKey))
        {
            throw new InternalException(ErrorCode.UNAUTHORIZED, "API Key was not provided.");
        }

        var apiKey = configuration.GetValue<string>(Configs.API_KEY_HEADER_NAME)!;
        if (!apiKey.Equals(extractedApiKey))
        {
            throw new InternalException(ErrorCode.UNAUTHORIZED, "Unauthorized client.");
        }

        await _next(context);
    }
}