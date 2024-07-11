using System.ComponentModel.DataAnnotations.Schema;

namespace BeerEconomy.DataStorageService.Database.Entities;

/// <summary>
///     Таблица пив
/// </summary>
[Table("beers")]
internal sealed class BeerEntity
{
    /// <summary>
    ///     Идентификатор В БД
    /// </summary>
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    ///     Названия пивы
    /// </summary>
    [Column("name")]
    public string Name { get; set; } = default!;
    
    /// <summary>
    ///     Описание пивы
    /// </summary>
    [Column("desc")]
    public string Description { get; set; } = default!;
    
    /// <summary>
    ///     Ссылка на изображение пивы
    /// </summary>
    [Column("image")]
    public string ImageUrl { get; set; } = default!;

    /// <summary>
    ///     Источники
    /// </summary>
    public ICollection<SourceEntity> Sources { get; set; } = new List<SourceEntity>();
    
    /// <summary>
    ///     Цены
    /// </summary>
    public ICollection<PriceEntity> Prices { get; set; } = new List<PriceEntity>();
}