using System.Text.Json.Serialization;

namespace BeerEconomy.Common.Models.Responses.Beers;

/// <summary>
///     Модель пива
/// </summary>
public sealed class BeerModel
{
    /// <summary>
    ///     Идентификатор В БД
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    ///     Названия пивы
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;
    
    /// <summary>
    ///     Описание пивы
    /// </summary>
    [JsonPropertyName("desc")]
    public string Description { get; set; } = default!;
    
    /// <summary>
    ///     Ссылка на изображение пивы
    /// </summary>
    [JsonPropertyName("image")]
    public string ImageUrl { get; set; } = default!;
}