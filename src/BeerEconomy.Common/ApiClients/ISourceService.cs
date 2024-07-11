using BeerEconomy.Common.Models.Requests.Sources;
using BeerEconomy.Common.Models.Responses.Sources;

namespace BeerEconomy.Common.ApiClients;

/// <summary>
///     Сервис источников
/// </summary>
public interface ISourceService
{
    /// <summary>
    ///     Получить список источников
    /// </summary>
    Task<List<SourceModel>> ListAsync(int beerId, CancellationToken cancellationToken);

    /// <summary>
    ///     Обновить источник
    /// </summary>
    Task<SourceModel> UpdateAsync(int id, UpdateSourceRequest request, CancellationToken cancellationToken);
    
    /// <summary>
    ///     Удалить источник
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken);
    
    /// <summary>
    ///     Создать источник
    /// </summary>
    Task<SourceModel> CreateAsync(AddSourceRequest request, CancellationToken cancellationToken);
}