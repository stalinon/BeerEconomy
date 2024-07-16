using System.Text;
using BeerEconomy.Common.Helpers.Exceptions;
using BeerEconomy.Common.Helpers.Logging;
using BeerEconomy.DataStorageService.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeerEconomy.DataStorageService.Database.Repositories.Impl;

/// <summary>
///     Репозиторий пива
/// </summary>  
internal sealed class BeerRepository(DataContext dataContext) : IRepository<BeerEntity>
{
    private static readonly ILogger<BeerRepository> Logger = StaticLogger.CreateLogger<BeerRepository>();
    
    /// <inheritdoc />
    public IQueryable<BeerEntity> CreateQuery()
    {
        return dataContext.Set<BeerEntity>()
            .Include(b => b.Prices)
            .Include(b => b.Sources);
    }

    /// <inheritdoc />
    public async Task<BeerEntity> GetAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await CreateQuery().FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        if (entity == null)
        {
            throw new InternalException(ErrorCode.NOT_FOUND, $"Не найдено пиво с id #{id}");
        }

        return entity;
    }

    /// <inheritdoc />
    public async Task<BeerEntity> UpdateAsync(int id, BeerEntity entity, CancellationToken cancellationToken)
    {
        var existingEntity = await GetAsync(id, cancellationToken);
        
        var log = new LogMessage($"Обновлена сущность пиво #{id}.");
        var updated = false;
        
        if (existingEntity.Name != entity.Name)
        {
            log = log.PropertyChanged(nameof(existingEntity.Name), existingEntity.Name, entity.Name);
            existingEntity.Name = entity.Name;
            updated = true;
        }
        
        if (existingEntity.Description != entity.Description)
        {
            log = log.PropertyChanged(nameof(existingEntity.Description), existingEntity.Description, entity.Description);
            existingEntity.Description = entity.Description;
            updated = true;
        }
        
        if (existingEntity.ImageUrl != entity.ImageUrl)
        {
            log = log.PropertyChanged(nameof(existingEntity.ImageUrl), existingEntity.ImageUrl, entity.ImageUrl);
            existingEntity.ImageUrl = entity.ImageUrl;
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
    public async Task<BeerEntity> CreateAsync(BeerEntity entity, CancellationToken cancellationToken)
    {
        await dataContext.AddAsync(entity, cancellationToken);
        await dataContext.SaveChangesAsync(cancellationToken);
        
        var log = new LogMessage($"Создана сущность пиво #{entity.Id}.");
        log = log.Property(nameof(entity.Name), entity.Name)
            .Property(nameof(entity.Description), entity.Description)
            .Property(nameof(entity.ImageUrl), entity.ImageUrl);
        Logger.LogInformation(log);
        
        return entity;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var beerCount = await dataContext.Set<BeerEntity>().Where(b => b.Id == id).ExecuteDeleteAsync(cancellationToken);
        if (beerCount == 0)
        {
            throw new InternalException(ErrorCode.NOT_FOUND, $"Не найдено пиво с id #{id}");
        }
        
        await dataContext.Set<SourceEntity>().Where(b => b.BeerId == id).ExecuteDeleteAsync(cancellationToken);
        await dataContext.Set<PriceEntity>().Where(b => b.BeerId == id).ExecuteDeleteAsync(cancellationToken);
        await dataContext.SaveChangesAsync(cancellationToken);
        
        var log = new LogMessage($"Удалена сущность пиво #{id} каскадом.");
        Logger.LogInformation(log);
    }
}