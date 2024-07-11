using BeerEconomy.Common;
using BeerEconomy.DataStorageService.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeerEconomy.DataStorageService.Database;

/// <summary>
///     Контекст БД
/// </summary>
internal sealed class DataContext : DbContext
{
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
    
    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Environment.GetEnvironmentVariable(Configs.CONNECTION_STRING)!;
        optionsBuilder.UseNpgsql(connectionString);
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BeerEntity>()
            .HasMany(b => b.Sources)
            .WithOne(b => b.Beer)
            .HasForeignKey(b => b.BeerId);
        modelBuilder.Entity<BeerEntity>()
            .HasMany(b => b.Prices)
            .WithOne(b => b.Beer)
            .HasForeignKey(b => b.BeerId);
    }
}