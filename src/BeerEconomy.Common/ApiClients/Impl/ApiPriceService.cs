using BeerEconomy.Common.Models.Requests.Prices;
using BeerEconomy.Common.Models.Responses;
using BeerEconomy.Common.Models.Responses.Prices;

namespace BeerEconomy.Common.ApiClients.Impl;

/// <summary>
///     Сервис цен
/// </summary>
internal sealed class ApiPriceService : ApiClientBase, IPriceService
{
    /// <inheritdoc />
    public ApiPriceService() : base("http://localhost:5080/api")
    {
    }

    /// <inheritdoc />
    public async Task<PagedList<PriceModel>> ListAsync(int beerId, ListPriceQuery query, CancellationToken cancellationToken)
    {
        var url = $"beers/{beerId}/prices";
        return await GetAsync<PagedList<PriceModel>>(url, query, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PriceModel> CreateAsync(AddPriceRequest request, CancellationToken cancellationToken)
    {
        var url = $"beers";
        return await PostAsync<AddPriceRequest, PriceModel>(url, request, cancellationToken);
    }
}