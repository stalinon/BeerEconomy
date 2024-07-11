using System.ComponentModel.DataAnnotations.Schema;

namespace BeerEconomy.DataStorageService.Database.Entities;

/// <summary>
///     Таблица цен
/// </summary>
[Table("prices")]
internal sealed class PriceEntity
{
    /// <summary>
    ///     Идентификатор в БД
    /// </summary>
    [Column("id")]
    public int Id { get; set; }
    
    /// <summary>
    ///     Временная метка
    /// </summary>
    [Column("date")]
    public DateOnly Date { get; set; }
    
    /// <summary>
    ///     Значение
    /// </summary>
    [Column("value")]
    public decimal Value { get; set; }
    
    /// <summary>
    ///     Идентификатор пивы в БД
    /// </summary>
    [Column("beer_id")]
    [ForeignKey(nameof(Beer))]
    public int BeerId { get; set; }
    
    /// <summary>
    ///     Идентификатор источника в БД
    /// </summary>
    [Column("source_id")]
    [ForeignKey(nameof(Source))]
    public int SourceId { get; set; }
    
    /// <summary>
    ///     Пиво
    /// </summary>
    public BeerEntity Beer { get; set; } = default!;

    /// <summary>
    ///     Источник
    /// </summary>
    public SourceEntity Source { get; set; } = default!;
}