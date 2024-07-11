using System.Text.Json.Serialization;
using BeerEconomy.Common.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace BeerEconomy.Common.Models.Requests;

/// <summary>
///     Пагинированный запрос
/// </summary>
public class PagedQuery : QueryBase
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
}