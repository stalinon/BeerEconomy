using BeerEconomy.Common.Models.Requests.Prices;
using BeerEconomy.Common.Models.Responses;
using BeerEconomy.Common.Models.Responses.Prices;
using BeerEconomy.DataStorageService.Database.Entities;
using BeerEconomy.DataStorageService.Database.Repositories.Impl;

namespace BeerEconomy.DataStorageService.Services.Impl;

/// <inheritdoc cref="IPriceService"/>
internal sealed class PriceService(PriceRepository priceRepository, BeerRepository beerRepository) : IPriceService
{
    /// <inheritdoc />
    public async Task<PagedList<PriceModel>> ListAsync(int beerId, ListPriceQuery query, CancellationToken cancellationToken)
    {
        var beer = await beerRepository.GetAsync(beerId, cancellationToken);
        var sourceId = beer.Sources.First(s => s.Source == query.Source).Id;
        
        var queryable = priceRepository.CreateQuery(beerId, sourceId, query.Timeframe)
            .Where(e => e.Date <= query.To && e.Date >= query.From)
            .Select(e => Map(e))
            .OrderByDescending(b => b.Date);
        return await PagedList<PriceModel>.PaginateAsync(queryable, query, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PriceModel> CreateAsync(AddPriceRequest request, CancellationToken cancellationToken)
    {
        var entity = await priceRepository.CreateAsync(new()
        {
            Date = request.Date,
            Value = request.Value,
            BeerId = request.BeerId,
            SourceId = request.SourceId
        }, cancellationToken);

        return Map(entity);
    }

    private PriceModel Map(PriceEntity entity) => new()
    {
        Date = entity.Date,
        Value = entity.Value
    };
}