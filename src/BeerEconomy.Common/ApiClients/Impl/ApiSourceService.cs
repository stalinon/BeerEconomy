using BeerEconomy.Common.Models.Requests.Sources;
using BeerEconomy.Common.Models.Responses.Sources;

namespace BeerEconomy.Common.ApiClients.Impl;

/// <summary>
///     Сервис источников
/// </summary>
internal sealed class ApiSourceService : ApiClientBase, ISourceService
{
    /// <inheritdoc />
    public ApiSourceService() : base(Environment.GetEnvironmentVariable(Configs.DATA_SERVICE_URL)!)
    {
    }

    /// <inheritdoc />
    public async Task<List<SourceModel>> ListAsync(int beerId, CancellationToken cancellationToken)
    {
        var url = $"beers/{beerId}/sources";
        return await GetAsync<List<SourceModel>>(url, null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SourceModel> UpdateAsync(int id, UpdateSourceRequest request, CancellationToken cancellationToken)
    {
        var url = $"sources/{id}";
        return await PutAsync<UpdateSourceRequest, SourceModel>(url, request, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var url = $"sources/{id}";
        await base.DeleteAsync(url, null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SourceModel> CreateAsync(AddSourceRequest request, CancellationToken cancellationToken)
    {
        var url = $"sources";
        return await PostAsync<AddSourceRequest, SourceModel>(url, request, cancellationToken);
    }
}