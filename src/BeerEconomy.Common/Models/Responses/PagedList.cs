using System.Text.Json.Serialization;
using BeerEconomy.Common.Models.Requests;
using Microsoft.EntityFrameworkCore;

namespace BeerEconomy.Common.Models.Responses;

/// <summary>
///     Пагинированный список
/// </summary>
public class PagedList<TItem>
{
    /// <summary>
    ///     Список элементов
    /// </summary>
    [JsonPropertyName("items")]
    public List<TItem> Items { get; init; } = new();

    /// <summary>
    ///     Общее количество
    /// </summary>
    [JsonPropertyName("total")]
    public int TotalCount { get; init; }

    /// <inheritdoc cref="PagedList{TItem}" />
    public PagedList()
    { }

    /// <summary>
    ///     Пагинировать
    /// </summary>
    public static async Task<PagedList<TItem>> PaginateAsync(
        IQueryable<TItem> queryable, PagedQuery query, CancellationToken cancellationToken = default)
    {
        var totalCount = await queryable.CountAsync(cancellationToken);
        var items = await queryable.Skip(query.Skip).Take(query.Max).ToListAsync(cancellationToken);
        return new()
        {
            Items = items,
            TotalCount = totalCount
        };
    }
}