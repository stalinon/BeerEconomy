using BeerEconomy.Common.Models.Requests.Prices;
using BeerEconomy.Common.Models.Responses;
using BeerEconomy.Common.Models.Responses.Prices;

namespace BeerEconomy.DataStorageService.Services;

/// <summary>
///     Сервис цен
/// </summary>
internal interface IPriceService
{
    /// <summary>
    ///     Получить список цен
    /// </summary>
    Task<PagedList<PriceModel>> ListAsync(int beerId, ListPriceQuery query, CancellationToken cancellationToken);
    
    /// <summary>
    ///     Добавить цену
    /// </summary>
    Task<PriceModel> CreateAsync(AddPriceRequest request, CancellationToken cancellationToken);
}