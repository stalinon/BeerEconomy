using System.Text.Json.Serialization;
using BeerEconomy.Common.Enums;

namespace BeerEconomy.Common.Models.Responses.Sources;

/// <summary>
///     Модель источника
/// </summary>
public sealed class SourceModel
{
    /// <summary>
    ///     Идентификатор В БД
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    /// <summary>
    ///     Тип источника
    /// </summary>
    [JsonPropertyName("source")]
    public Source Source { get; set; }

    /// <summary>
    ///     Ссылка
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = default!;
}