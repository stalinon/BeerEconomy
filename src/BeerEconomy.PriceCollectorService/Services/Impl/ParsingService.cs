using BeerEconomy.Common.ApiClients;
using BeerEconomy.Common.Enums;
using BeerEconomy.Common.Models.Responses.Beers;
using BeerEconomy.Common.Models.Responses.Sources;

namespace BeerEconomy.PriceCollectorService.Services.Impl;

/// <inheritdoc cref="IParsingService" />
internal sealed class ParsingService(
        IEnumerable<IParsingService> parsingServices,
        IBeerService beerService,
        ISourceService sourceService)
    : IParsingService
{
    private const int PageSize = 10_000;

    /// <inheritdoc />
    public Source? Type => null;

    /// <inheritdoc />
    public async Task ParsePricesAsync(IDictionary<int, SourceModel> sources, CancellationToken cancellationToken)
    {
        var beers = await LoadBeersAsync(cancellationToken);
        var sourceLists = await LoadSourcesAsync(beers, cancellationToken);

        var tasks = parsingServices.Select(ParseSingleSource).ToArray();
        Task.WaitAll(tasks, cancellationToken);

        Task ParseSingleSource(IParsingService service)
        {
            sources = sourceLists.Where(sl
                    => sl.Value.Any(s => s.Source == service.Type))
                .ToDictionary(
                    sl => sl.Key,
                    sl => sl.Value.First(v => v.Source == service.Type));
            return service.ParsePricesAsync(sources, cancellationToken);
        }
    }

    private async Task<Dictionary<int, List<SourceModel>>> LoadSourcesAsync(List<BeerModel> beers, CancellationToken cancellationToken)
    {
        var sourceLists = new Dictionary<int, List<SourceModel>>();
        foreach (var beer in beers)
        {
            sourceLists[beer.Id] = await sourceService.ListAsync(beer.Id, cancellationToken);
        }

        return sourceLists;
    }

    private async Task<List<BeerModel>> LoadBeersAsync(CancellationToken cancellationToken)
    {
        var beerPages = await beerService.ListAsync(new() {Max = PageSize}, cancellationToken);
        var beers = beerPages.Items;
        while (beers.Count < beerPages.TotalCount)
        {
            beerPages = await beerService.ListAsync(new() {Skip = beers.Count, Max = PageSize}, cancellationToken);
            beers.AddRange(beers);
        }

        return beers;
    }
}