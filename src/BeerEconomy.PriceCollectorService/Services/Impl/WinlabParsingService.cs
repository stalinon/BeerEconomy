using BeerEconomy.Common.ApiClients;
using BeerEconomy.Common.Enums;
using BeerEconomy.Common.Models.Requests.Prices;
using BeerEconomy.Common.Models.Responses.Sources;
using HtmlAgilityPack;

namespace BeerEconomy.PriceCollectorService.Services.Impl;

/// <inheritdoc cref="IParsingService" />
internal sealed class WinlabParsingService(IPriceService service) : IParsingService
{
    /// <inheritdoc />
    public Source? Type => Source.WINLAB;

    /// <inheritdoc />
    public async Task ParsePricesAsync(IDictionary<int, SourceModel> sources, CancellationToken cancellationToken)
    {
        foreach (var (beerId, source) in sources)
        {
            try
            {
                var httpClient = new HttpClient();
                var html = await httpClient.GetStringAsync(source.Url, cancellationToken);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var oldPriceNode = doc.DocumentNode.SelectSingleNode("//span[contains(@class, 'js-catalog-item-price') and contains(@class, 'price-old')]");
                if (oldPriceNode == null)
                {
                    throw new Exception($"Old price not found for Beer ID: {beerId}");
                }

                var oldPriceText = oldPriceNode.GetAttributeValue("data-price", string.Empty);
                if (!decimal.TryParse(oldPriceText, out var oldPrice))
                {
                    continue;
                }

                Console.WriteLine($"Beer ID: {beerId}, Old Price: {oldPrice}");

                var request = new AddPriceRequest
                {
                    BeerId = beerId,
                    SourceId = source.Id,
                    Date = DateOnly.FromDateTime(DateTime.UtcNow),
                    Value = oldPrice
                };

                await service.CreateAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to parse price for Beer ID: {beerId}. Error: {ex.Message}");
            }
        }
    }
}