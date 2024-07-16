using BeerEconomy.Common.Enums;
using BeerEconomy.Common.Helpers.Exceptions;
using BeerEconomy.Common.Helpers.Logging;
using BeerEconomy.DataStorageService.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeerEconomy.DataStorageService.Database.Repositories.Impl;

/// <summary>
///     Репозиторий цены
/// </summary>  
internal sealed class PriceRepository(DataContext dataContext) : IRepository<PriceEntity>
{
    private static readonly ILogger<PriceRepository> Logger = StaticLogger.CreateLogger<PriceRepository>();
    
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
        var entity = await CreateQuery().FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        if (entity == null)
        {
            throw new InternalException(ErrorCode.NOT_FOUND, $"Не найдена цена с id #{id}");
        }

        return entity;
    }

    /// <inheritdoc />
    public async Task<PriceEntity> UpdateAsync(int id, PriceEntity entity, CancellationToken cancellationToken)
    {
        var existingEntity = await GetAsync(id, cancellationToken);
        
        var log = new LogMessage($"Обновлена сущность цена #{id}.");
        var updated = false;
        
        if (existingEntity.Date != entity.Date)
        {
            log = log.PropertyChanged(nameof(existingEntity.Date), existingEntity.Date, entity.Date);
            existingEntity.Date = entity.Date;
            updated = true;
        }
        
        if (existingEntity.Value != entity.Value)
        {
            log = log.PropertyChanged(nameof(existingEntity.Value), existingEntity.Value, entity.Value);
            existingEntity.Value = entity.Value;
            updated = true;
        }

        if (!updated)
        {
            return existingEntity;
        }
        
        await dataContext.SaveChangesAsync(cancellationToken);
        
        Logger.LogInformation(log);

        return existingEntity;
    }

    /// <inheritdoc />
    public async Task<PriceEntity> CreateAsync(PriceEntity entity, CancellationToken cancellationToken)
    {
        if (dataContext.Prices.Any(p =>
                p.BeerId == entity.BeerId && p.SourceId == entity.SourceId && p.Date == entity.Date))
        {
            throw new InternalException(ErrorCode.CONFLICT, $"Цена уже существует.");
        }
        
        await dataContext.AddAsync(entity, cancellationToken);
        await dataContext.SaveChangesAsync(cancellationToken);
        
        var log = new LogMessage($"Создана сущность цена #{entity.Id}.");
        log = log.Property(nameof(entity.BeerId), entity.BeerId)
            .Property(nameof(entity.Date), entity.Date)
            .Property(nameof(entity.Value), entity.Value);
        Logger.LogInformation(log);
        
        return entity;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var priceCount = await dataContext.Set<PriceEntity>().Where(b => b.Id == id).ExecuteDeleteAsync(cancellationToken);
        if (priceCount == 0)
        {
            throw new InternalException(ErrorCode.NOT_FOUND, $"Не найдена цена с id #{id}");
        }
        
        await dataContext.SaveChangesAsync(cancellationToken);
        
        var log = new LogMessage($"Удалена сущность цена #{id}.");
        Logger.LogInformation(log);
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