using System.Text.Json.Serialization;

namespace BeerEconomy.Common.Models.Requests.Prices;

/// <summary>
///     Добавить цену
/// </summary>
public sealed class AddPriceRequest
{
    /// <summary>
    ///     Идентификатор пива
    /// </summary>
    [JsonPropertyName("beer_id")]
    public int BeerId { get; set; }
    
    /// <summary>
    ///     Идентификатор источника
    /// </summary>
    [JsonPropertyName("source_id")]
    public int SourceId { get; set; }
    
    /// <summary>
    ///     Временная метка
    /// </summary>
    [JsonPropertyName("date")]
    public DateOnly Date { get; set; }
    
    /// <summary>
    ///     Значение цены
    /// </summary>
    [JsonPropertyName("value")]
    public decimal Value { get; set; }
}