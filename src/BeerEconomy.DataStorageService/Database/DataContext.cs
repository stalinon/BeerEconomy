using BeerEconomy.DataStorageService.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeerEconomy.DataStorageService.Database;

/// <summary>
///     Контекст БД
/// </summary>
internal sealed class DataContext : DbContext
{
    /// <inheritdoc cref="DataContext" />
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    /// <summary>
    ///     Пивы
    /// </summary>
    public DbSet<BeerEntity> Beers { get; set; } = null!;
    
    /// <summary>
    ///     Источники
    /// </summary>
    public DbSet<SourceEntity> Sources { get; set; } = null!;
    
    /// <summary>
    ///     Цены
    /// </summary>
    public DbSet<PriceEntity> Prices { get; set; } = null!;
}