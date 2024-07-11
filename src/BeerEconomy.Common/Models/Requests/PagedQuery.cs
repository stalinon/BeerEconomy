using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace BeerEconomy.Common.Models.Requests;

/// <summary>
///     Пагинированный запрос
/// </summary>
public class PagedQuery
{
    /// <summary>
    ///     Сдвиг
    /// </summary>
    [JsonPropertyName("skip")]
    [FromQuery(Name = "skip")]
    public int Skip { get; set; } = 0;

    /// <summary>
    ///     Максимальное количество элементов на странице
    /// </summary>
    [JsonPropertyName("max")]
    [FromQuery(Name = "max")]
    public int Max { get; set; } = 20;

    /// <summary>
    ///     Следующий
    /// </summary>
    [JsonIgnore]
    public PagedQuery Next => new()
    {
        Skip = Skip + Max,
        Max = Max
    };
}