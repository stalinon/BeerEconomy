using System.Collections;
using BeerEconomy.Common.Models.Requests;
using Microsoft.EntityFrameworkCore;

namespace BeerEconomy.Common.Models.Responses;

/// <summary>
///     Пагинированный список
/// </summary>
public class PagedList<TItem> : IEnumerable<TItem>
{
    /// <summary>
    ///     Список элементов
    /// </summary>
    public List<TItem> Items { get; init; } = new();

    /// <summary>
    ///     Общее количество
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    ///     Следующий запрос
    /// </summary>
    public PagedQuery Next { get; init; } = new();

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
            TotalCount = totalCount,
            Next = query.Next
        };
    }

    /// <inheritdoc />
    public IEnumerator<TItem> GetEnumerator()
    {
        return Items.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable) Items).GetEnumerator();
    }
}