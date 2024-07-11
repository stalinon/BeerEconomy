using System.ComponentModel.DataAnnotations.Schema;
using BeerEconomy.Common.Enums;

namespace BeerEconomy.DataStorageService.Database.Entities;

/// <summary>
///     Таблица источников
/// </summary>
[Table("sources")]
internal sealed class SourceEntity
{
    /// <summary>
    ///     Идентификатор в БД
    /// </summary>
    [Column("id")]
    public int Id { get; set; }
    
    /// <summary>
    ///     Тип источника
    /// </summary>
    [Column("source")]
    public Source Source { get; set; }

    /// <summary>
    ///     Ссылка
    /// </summary>
    [Column("url")]
    public string Url { get; set; } = default!;
    
    /// <summary>
    ///     Идентификатор пива
    /// </summary>
    [Column("beer_id")]
    [ForeignKey(nameof(Beer))]
    public int BeerId { get; set; }

    /// <summary>
    ///     Пиво
    /// </summary>
    public BeerEntity Beer { get; set; } = default!;
}