using BeerEconomy.Common.Models.Requests.Beers;
using BeerEconomy.Common.Models.Responses;
using BeerEconomy.Common.Models.Responses.Beers;
using BeerEconomy.DataStorageService.Database.Entities;
using BeerEconomy.DataStorageService.Database.Repositories.Impl;

namespace BeerEconomy.DataStorageService.Services.Impl;

/// <inheritdoc cref="IBeerService"/>
internal sealed class BeerService(BeerRepository beerRepository) : IBeerService
{
    /// <inheritdoc />
    public async Task<BeerModel> GetAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await beerRepository.GetAsync(id, cancellationToken);
        return Map(entity);
    }

    /// <inheritdoc />
    public async Task<PagedList<BeerModel>> ListAsync(ListBeerQuery query, CancellationToken cancellationToken)
    {
        var queryable = beerRepository.CreateQuery()
            .Select(e => new BeerModel
            {
                Id = e.Id,
                Description = e.Description,
                ImageUrl = e.ImageUrl,
                Name = e.Name
            })
            .OrderBy(b => b.Id);
        return await PagedList<BeerModel>.PaginateAsync(queryable, query, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<BeerModel> UpdateAsync(int id, UpdateBeerRequest request, CancellationToken cancellationToken)
    {
        var entity = await beerRepository.UpdateAsync(id, new()
        {
            Name = request.Name,
            Description = request.Description,
            ImageUrl = request.ImageUrl
        }, cancellationToken);

        return Map(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await beerRepository.DeleteAsync(id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<BeerModel> CreateAsync(AddBeerRequest request, CancellationToken cancellationToken)
    {
        var entity = await beerRepository.CreateAsync(new()
        {
            Name = request.Name,
            Description = request.Description,
            ImageUrl = request.ImageUrl
        }, cancellationToken);

        return Map(entity);
    }

    private BeerModel Map(BeerEntity entity) => new()
    {
        Id = entity.Id,
        Description = entity.Description,
        ImageUrl = entity.ImageUrl,
        Name = entity.Name
    };
}