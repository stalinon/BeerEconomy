using BeerEconomy.Common.Models.Responses.Sources;
using BeerEconomy.PriceCollectorService.Services;
using BeerEconomy.PriceCollectorService.Services.Impl;
using Microsoft.AspNetCore.Mvc;

namespace BeerEconomy.PriceCollectorService.Controllers;

/// <summary>
///     Контроллер парсинга
/// </summary>
[ApiController]
[Route("~/api/parse")]
public sealed class ParseController(ParsingService parsingService) : Controller
{
    /// <summary>
    ///     Запустить парсинг
    /// </summary>
    [HttpPost]
    public Task<IActionResult> StartParsing(
        CancellationToken cancellationToken)
    {
        parsingService.ParsePricesAsync(new Dictionary<int, SourceModel>(), cancellationToken);
        return Task.FromResult<IActionResult>(Ok());
    }
}