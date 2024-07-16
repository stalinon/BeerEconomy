using System.Text.Json.Serialization;

namespace BeerEconomy.Common.Helpers.Exceptions;

/// <summary>
///     Модель ошибки
/// </summary>
public sealed class Error
{
    /// <summary>
    ///     Код ошибки
    /// </summary>
    [JsonPropertyName("code")]
    public ErrorCode Code { get; set; }

    /// <summary>
    ///     Сообщение ошибки
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = default!;
}