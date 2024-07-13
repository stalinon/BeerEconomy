using BeerEconomy.Common.Enums;
using BeerEconomy.Common.Models.Responses.Sources;

namespace BeerEconomy.PriceCollectorService.Services;

/// <summary>
///     Сервис парсинга цен
/// </summary>
public interface IParsingService
{
    /// <summary>
    ///     Тип сервиса парсинга
    /// </summary>
    Source? Type { get; }

    /// <summary>
    ///     Спарсить цены из источника
    /// </summary>
    /// <param name="sources">Соответствие (id пива)=>(url страницы для парса)</param>
    /// <param name="cancellationToken"></param>
    Task ParsePricesAsync(IDictionary<int, SourceModel> sources, CancellationToken cancellationToken);
}