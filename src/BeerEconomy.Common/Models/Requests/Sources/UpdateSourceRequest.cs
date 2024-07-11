using System.Text.Json.Serialization;
using BeerEconomy.Common.Enums;

namespace BeerEconomy.Common.Models.Requests.Sources;

/// <summary>
///     Обновить источник к пиву
/// </summary>
public class UpdateSourceRequest
{
    /// <summary>
    ///     Идентификатор пива
    /// </summary>
    [JsonPropertyName("beer_id")]
    public int BeerId { get; set; }

    /// <summary>
    ///     Ссылка
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = default!;
}