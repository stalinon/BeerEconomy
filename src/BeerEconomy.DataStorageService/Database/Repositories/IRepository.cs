namespace BeerEconomy.DataStorageService.Database.Repositories;

/// <summary>
///     Репозиторий
/// </summary>
internal interface IRepository<TEntity>
    where TEntity : class
{
    /// <summary>
    ///     Получить основу запроса
    /// </summary>
    IQueryable<TEntity> CreateQuery();

    /// <summary>
    ///     Получить по идентификатору
    /// </summary>
    Task<TEntity> GetAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    ///     Обновить сущность
    /// </summary>
    Task<TEntity> UpdateAsync(int id, TEntity entity, CancellationToken cancellationToken);

    /// <summary>
    ///     Создать сущность
    /// </summary>
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken);

    /// <summary>
    ///     Удалить сущность
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}