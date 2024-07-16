using BeerEconomy.Common.Helpers.Exceptions;
using BeerEconomy.Common.Helpers.Logging;
using BeerEconomy.DataStorageService.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeerEconomy.DataStorageService.Database.Repositories.Impl;

/// <summary>
///     Репозиторий источников
/// </summary>  
internal sealed class SourceRepository(DataContext dataContext) : IRepository<SourceEntity>
{
    private static readonly ILogger<SourceRepository> Logger = StaticLogger.CreateLogger<SourceRepository>();
    
    /// <inheritdoc />
    public IQueryable<SourceEntity> CreateQuery()
    {
        return dataContext.Set<SourceEntity>()
            .Include(s => s.Beer);
    }

    /// <inheritdoc />
    public async Task<SourceEntity> GetAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await CreateQuery().FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        if (entity == null)
        {
            throw new InternalException(ErrorCode.NOT_FOUND, $"Не найден источник с id #{id}");
        }

        return entity;
    }

    /// <inheritdoc />
    public async Task<SourceEntity> UpdateAsync(int id, SourceEntity entity, CancellationToken cancellationToken)
    {
        var existingEntity = await GetAsync(id, cancellationToken);
        var log = new LogMessage($"Обновлена сущность источник #{id}.");
        var updated = false;
        
        if (existingEntity.Url != entity.Url)
        {
            log = log.PropertyChanged(nameof(existingEntity.Url), existingEntity.Url, entity.Url);
            existingEntity.Url = entity.Url;
            updated = true;
        }
        
        if (existingEntity.Source != entity.Source)
        {
            log = log.PropertyChanged(nameof(existingEntity.Source), existingEntity.Source, entity.Source);
            existingEntity.Source = entity.Source;
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
    public async Task<SourceEntity> CreateAsync(SourceEntity entity, CancellationToken cancellationToken)
    {
        await dataContext.AddAsync(entity, cancellationToken);
        await dataContext.SaveChangesAsync(cancellationToken);
        
        var log = new LogMessage($"Создана сущность источник #{entity.Id}.");
        log = log.Property(nameof(entity.BeerId), entity.BeerId)
            .Property(nameof(entity.Source), entity.Source)
            .Property(nameof(entity.Url), entity.Url);
        Logger.LogInformation(log);
        
        return entity;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var sourceCount = await dataContext.Set<SourceEntity>().Where(b => b.Id == id).ExecuteDeleteAsync(cancellationToken);
        if (sourceCount == 0)
        {
            throw new InternalException(ErrorCode.NOT_FOUND, $"Не найден источник с id #{id}");
        }
        
        await dataContext.SaveChangesAsync(cancellationToken);
        
        var log = new LogMessage($"Удалена сущность источник #{id}.");
        Logger.LogInformation(log);
    }
}