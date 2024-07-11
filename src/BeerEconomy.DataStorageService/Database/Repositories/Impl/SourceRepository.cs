using BeerEconomy.DataStorageService.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeerEconomy.DataStorageService.Database.Repositories.Impl;

/// <summary>
///     Репозиторий источников
/// </summary>  
internal sealed class SourceRepository(DataContext dataContext) : IRepository<SourceEntity>
{
    /// <inheritdoc />
    public IQueryable<SourceEntity> CreateQuery()
    {
        return dataContext.Set<SourceEntity>()
            .Include(s => s.Beer);
    }

    /// <inheritdoc />
    public async Task<SourceEntity> GetAsync(int id, CancellationToken cancellationToken)
    {
        return await CreateQuery().FirstAsync(b => b.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SourceEntity> UpdateAsync(int id, SourceEntity entity, CancellationToken cancellationToken)
    {
        var existingEntity = await GetAsync(id, cancellationToken);
        existingEntity.Url = entity.Url;
        existingEntity.Source = entity.Source;
        await dataContext.SaveChangesAsync(cancellationToken);

        return existingEntity;
    }

    /// <inheritdoc />
    public async Task<SourceEntity> CreateAsync(SourceEntity entity, CancellationToken cancellationToken)
    {
        await dataContext.AddAsync(entity, cancellationToken);
        return entity;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await dataContext.Set<SourceEntity>().Where(b => b.Id == id).ExecuteDeleteAsync(cancellationToken);
    }
}