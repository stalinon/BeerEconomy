using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BeerEconomy.Common.Helpers.Exceptions;

/// <summary>
///     Мидлварь для отлова ошибок
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    /// <inheritdoc cref="ExceptionMiddleware"/>
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc cref="ExceptionMiddleware"/>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (InternalException ex)
        {
            _logger.LogWarning(ex, "Произошло обработанное исключение.");
            await HandleExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошло необработанное исключение.");
            await HandleExceptionAsync(context, new InternalException(ErrorCode.INTERNAL_SERVER_ERROR, "Внутренняя ошибка сервера", ex));
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, InternalException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)exception.ErrorCode;

        var response = new Error
        {
            Code = exception.ErrorCode,
            Message = exception.Message
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}