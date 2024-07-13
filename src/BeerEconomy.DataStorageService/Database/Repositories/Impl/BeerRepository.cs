using BeerEconomy.DataStorageService.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeerEconomy.DataStorageService.Database.Repositories.Impl;

/// <summary>
///     Репозиторий пива
/// </summary>  
internal sealed class BeerRepository(DataContext dataContext) : IRepository<BeerEntity>
{
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
        return await CreateQuery().FirstAsync(b => b.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<BeerEntity> UpdateAsync(int id, BeerEntity entity, CancellationToken cancellationToken)
    {
        var existingEntity = await GetAsync(id, cancellationToken);
        existingEntity.Description = entity.Description;
        existingEntity.ImageUrl = entity.ImageUrl;
        existingEntity.Name = entity.Name;
        await dataContext.SaveChangesAsync(cancellationToken);

        return existingEntity;
    }

    /// <inheritdoc />
    public async Task<BeerEntity> CreateAsync(BeerEntity entity, CancellationToken cancellationToken)
    {
        await dataContext.AddAsync(entity, cancellationToken);
        await dataContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await dataContext.Set<BeerEntity>().Where(b => b.Id == id).ExecuteDeleteAsync(cancellationToken);
        await dataContext.Set<SourceEntity>().Where(b => b.BeerId == id).ExecuteDeleteAsync(cancellationToken);
        await dataContext.Set<PriceEntity>().Where(b => b.BeerId == id).ExecuteDeleteAsync(cancellationToken);
        await dataContext.SaveChangesAsync(cancellationToken);
    }
}