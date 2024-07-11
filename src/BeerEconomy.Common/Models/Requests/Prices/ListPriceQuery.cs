using BeerEconomy.Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace BeerEconomy.Common.Models.Requests.Prices;

/// <summary>
///     Получить список цен
/// </summary>
public sealed class ListPriceQuery : PagedQuery
{
    /// <summary>
    ///     Начало фильтрации
    /// </summary>
    [FromQuery(Name = "from")]
    public DateOnly From { get; set; }

    /// <summary>
    ///     Конец фильтрации
    /// </summary>
    [FromQuery(Name = "to")]
    public DateOnly To { get; set; }

    /// <summary>
    ///     Источник
    /// </summary>
    [FromQuery(Name = "src")]
    public Source Source { get; set; }

    /// <summary>
    ///     Таймфрейм
    /// </summary>
    [FromQuery(Name = "timeframe")]
    public Timeframe Timeframe { get; set; }
}