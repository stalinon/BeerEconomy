using BeerEconomy.Common.ApiClients;
using BeerEconomy.Common.Enums;
using BeerEconomy.Common.Helpers.Exceptions;
using BeerEconomy.Common.Helpers.Logging;
using BeerEconomy.Common.Models.Responses.Sources;
using HtmlAgilityPack;

namespace BeerEconomy.PriceCollectorService.Services.Impl;

/// <summary>
///     Сервис парсинга цен с сайта <see cref="Source.PEREKRESTOK"/>
/// </summary>
internal sealed class PerekrestokParsingService(IPriceService priceService) : IParsingService
{
    private static readonly ILogger<PerekrestokParsingService> Logger =
        StaticLogger.CreateLogger<PerekrestokParsingService>();
    
    /// <inheritdoc />
    public Source? Type => Source.PEREKRESTOK;

    /// <inheritdoc />
    public async Task ParsePricesAsync(IDictionary<int, SourceModel> sources, CancellationToken cancellationToken)
    {
        foreach (var (beerId, source) in sources)
        {
            try
            {
                var price = await GetPriceFromUrlAsync(source.Url, cancellationToken);
                await priceService.CreateAsync(new()
                {
                    BeerId = beerId,
                    SourceId = source.Id,
                    Value = price,
                    Date = DateOnly.FromDateTime(DateTime.UtcNow)
                }, cancellationToken);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, $"Не удалось спарсить цену для пива #{beerId} из источника {source.Source}.");
            } 
        }
    }

    private async Task<decimal> GetPriceFromUrlAsync(string url, CancellationToken cancellationToken)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetStringAsync(url, cancellationToken);
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(response);

        var priceNode = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class, 'price-new')]/span[contains(@class, 'sr-only')]");
        if (priceNode != null && decimal.TryParse(
            priceNode.InnerText.Replace("&nbsp;", "").Replace("₽", "").Trim(),
            out var price))
        {
            return price;
        }

        throw new InternalException(ErrorCode.NOT_FOUND, $"Не найдена цена на странице.");
    }
}