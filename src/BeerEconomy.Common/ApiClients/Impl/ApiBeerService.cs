using BeerEconomy.Common.Models.Requests.Beers;
using BeerEconomy.Common.Models.Responses;
using BeerEconomy.Common.Models.Responses.Beers;

namespace BeerEconomy.Common.ApiClients.Impl;

/// <summary>
///     Сервис пив
/// </summary>
internal sealed class ApiBeerService : ApiClientBase, IBeerService
{
    /// <inheritdoc />
    public ApiBeerService() : base("http://localhost:5080")
    {
    }

    /// <inheritdoc />
    public async Task<BeerModel> GetAsync(int id, CancellationToken cancellationToken)
    {
        var url = $"beers/{id}";
        return await GetAsync<BeerModel>(url, null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PagedList<BeerModel>> ListAsync(ListBeerQuery query, CancellationToken cancellationToken)
    {
        var url = $"beers";
        return await GetAsync<PagedList<BeerModel>>(url, query, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<BeerModel> UpdateAsync(int id, UpdateBeerRequest request, CancellationToken cancellationToken)
    {
        var url = $"beers/{id}";
        return await PutAsync<UpdateBeerRequest, BeerModel>(url, request, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var url = $"beers/{id}";
        await base.DeleteAsync(url, null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<BeerModel> CreateAsync(AddBeerRequest request, CancellationToken cancellationToken)
    {
        var url = $"beers";
        return await PostAsync<AddBeerRequest, BeerModel>(url, request, cancellationToken);
    }
}