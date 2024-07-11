using System.Text.Json.Serialization;

namespace BeerEconomy.Common.Models.Responses.Prices;

/// <summary>
///     Модель пива
/// </summary>
public sealed class PriceModel
{
    /// <summary>
    ///     Временная метка
    /// </summary>
    [JsonPropertyName("date")]
    public DateOnly Date { get; set; }
    
    /// <summary>
    ///     Значение
    /// </summary>
    [JsonPropertyName("value")]
    public decimal Value { get; set; }
}