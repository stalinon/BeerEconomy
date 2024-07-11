using BeerEconomy.Common.Models.Requests.Beers;
using BeerEconomy.Common.Models.Responses;
using BeerEconomy.Common.Models.Responses.Beers;

namespace BeerEconomy.Common.ApiClients;

/// <summary>
///     Сервис пив
/// </summary>
public interface IBeerService
{
    /// <summary>
    ///     Получить пива
    /// </summary>
    Task<BeerModel> GetAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    ///     Получить список пив
    /// </summary>
    Task<PagedList<BeerModel>> ListAsync(ListBeerQuery query, CancellationToken cancellationToken);

    /// <summary>
    ///     Обновить пиву
    /// </summary>
    Task<BeerModel> UpdateAsync(int id, UpdateBeerRequest request, CancellationToken cancellationToken);
    
    /// <summary>
    ///     Удалить пиву
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken);
    
    /// <summary>
    ///     Создать пиву
    /// </summary>
    Task<BeerModel> CreateAsync(AddBeerRequest request, CancellationToken cancellationToken);
}