using System.Text.Json.Serialization;

namespace BeerEconomy.Common.Models.Requests.Beers;

/// <summary>
///     Добавить пиво
/// </summary>
public sealed class AddBeerRequest
{
    /// <summary>
    ///     Название
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;
    
    /// <summary>
    ///     Описание
    /// </summary>
    [JsonPropertyName("desc")]
    public string Description { get; set; } = default!;
    
    /// <summary>
    ///     Ссылка на изображение
    /// </summary>
    [JsonPropertyName("image_url")]
    public string ImageUrl { get; set; } = default!;
}