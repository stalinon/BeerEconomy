using BeerEconomy.Common.Enums;
using BeerEconomy.DataStorageService.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeerEconomy.DataStorageService.Database.Repositories.Impl;

/// <summary>
///     Репозиторий цены
/// </summary>  
internal sealed class PriceRepository(DataContext dataContext) : IRepository<PriceEntity>
{
    /// <inheritdoc />
    public IQueryable<PriceEntity> CreateQuery()
    {
        return dataContext.Set<PriceEntity>()
            .Include(p => p.Source)
            .Include(p => p.Beer);
    }

    /// <inheritdoc />
    public IQueryable<PriceEntity> CreateQuery(int beerId, int sourceId, Timeframe timeframe)
    {
        var queryable = dataContext.Set<PriceEntity>()
            .FromSqlRaw(AggregationSql(timeframe))
            .Where(e => e.BeerId == beerId && e.SourceId == sourceId);

        return queryable;
    }

    /// <inheritdoc />
    public async Task<PriceEntity> GetAsync(int id, CancellationToken cancellationToken)
    {
        return await CreateQuery().FirstAsync(b => b.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PriceEntity> UpdateAsync(int id, PriceEntity entity, CancellationToken cancellationToken)
    {
        var existingEntity = await GetAsync(id, cancellationToken);
        existingEntity.Date = entity.Date;
        existingEntity.Value = entity.Value;
        await dataContext.SaveChangesAsync(cancellationToken);

        return existingEntity;
    }

    /// <inheritdoc />
    public async Task<PriceEntity> CreateAsync(PriceEntity entity, CancellationToken cancellationToken)
    {
        await dataContext.AddAsync(entity, cancellationToken);
        await dataContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await dataContext.Set<PriceEntity>().Where(b => b.Id == id).ExecuteDeleteAsync(cancellationToken);
        await dataContext.SaveChangesAsync(cancellationToken);
    }

    private string AggregationSql(Timeframe timeframe)
    {
        return timeframe switch
        {
            Timeframe.DAY => "SELECT min(id), min(\"date\"), avg(value), beer_id, source_id FROM prices GROUP BY beer_id, source_id, DATE_TRUNC('day', \"date\")",
            Timeframe.WEEK => "SELECT min(id), min(\"date\"), avg(value), beer_id, source_id FROM prices GROUP BY beer_id, source_id, DATE_TRUNC('week', \"date\")",
            Timeframe.MONTH => "SELECT min(id), min(\"date\"), avg(value), beer_id, source_id FROM prices GROUP BY beer_id, source_id, DATE_TRUNC('month', \"date\")",
            Timeframe.YEAR => "SELECT min(id), min(\"date\"), avg(value), beer_id, source_id FROM prices GROUP BY beer_id, source_id, DATE_TRUNC('year', \"date\")",
            _ => throw new ArgumentOutOfRangeException(nameof(timeframe), timeframe, null)
        };
    }
}