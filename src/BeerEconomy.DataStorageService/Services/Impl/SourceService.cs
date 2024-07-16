using BeerEconomy.Common.ApiClients;
using BeerEconomy.Common.Models.Requests.Sources;
using BeerEconomy.Common.Models.Responses.Sources;
using BeerEconomy.DataStorageService.Database.Entities;
using BeerEconomy.DataStorageService.Database.Repositories.Impl;

namespace BeerEconomy.DataStorageService.Services.Impl;

/// <inheritdoc cref="ISourceService"/>
internal sealed class SourceService(SourceRepository sourceRepository, BeerRepository beerRepository) : ISourceService
{
    /// <inheritdoc />
    public async Task<List<SourceModel>> ListAsync(int beerId, CancellationToken cancellationToken)
    {
        var beer = await beerRepository.GetAsync(beerId, cancellationToken);
        return beer.Sources.Select(Map).ToList();
    }

    /// <inheritdoc />
    public async Task<SourceModel> UpdateAsync(int id, UpdateSourceRequest request, CancellationToken cancellationToken)
    {
        var entity = await sourceRepository.UpdateAsync(id, new()
        {
            Url = request.Url
        }, cancellationToken);

        return Map(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await sourceRepository.DeleteAsync(id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SourceModel> CreateAsync(AddSourceRequest request, CancellationToken cancellationToken)
    {
        var entity = await sourceRepository.CreateAsync(new()
        {
            Url = request.Url,
            Source = request.Source,
            BeerId = request.BeerId
        }, cancellationToken);

        return Map(entity);
    }

    private SourceModel Map(SourceEntity entity) => new()
    {
        Id = entity.Id,
        Source = entity.Source,
        Url = entity.Url
    };
}