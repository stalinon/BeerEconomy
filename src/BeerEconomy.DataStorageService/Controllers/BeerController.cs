using BeerEconomy.Common.Models.Requests.Beers;
using BeerEconomy.Common.Models.Requests.Prices;
using BeerEconomy.DataStorageService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BeerEconomy.DataStorageService.Controllers;

/// <summary>
///     Контроллер пив
/// </summary>
[ApiController]
[Route("~/api/beers")]
public sealed class BeerController : Controller
{
    private readonly IBeerService _beerService;
    private readonly IPriceService _priceService;
    private readonly ISourceService _sourceService;

    /// <inheritdoc href="BeerController" />
    public BeerController(
        IBeerService beerService,
        IPriceService priceService,
        ISourceService sourceService)
    {
        _beerService = beerService;
        _priceService = priceService;
        _sourceService = sourceService;
    }

    /// <summary>
    ///     Получить список пив
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] ListBeerQuery query,
        CancellationToken cancellationToken)
    {
        var beers = await _beerService.ListAsync(query, cancellationToken);
        return Ok(beers);
    }

    /// <summary>
    ///     Получить пиву
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var beer = await _beerService.GetAsync(id, cancellationToken);
        return Ok(beer);
    }

    /// <summary>
    ///     Создать пиву
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] AddBeerRequest request,
        CancellationToken cancellationToken)
    {
        var beer = await _beerService.CreateAsync(request, cancellationToken);
        return Ok(beer);
    }

    /// <summary>
    ///     Обновить пиву
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        [FromRoute] int id,
        [FromBody] UpdateBeerRequest request,
        CancellationToken cancellationToken)
    {
        var beer = await _beerService.UpdateAsync(id, request, cancellationToken);
        return Ok(beer);
    }

    /// <summary>
    ///     Удалить пиву
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await _beerService.DeleteAsync(id, cancellationToken);
        return Ok();
    }

    /// <summary>
    ///     Получить цены
    /// </summary>
    [HttpGet("{id}/prices")]
    public async Task<IActionResult> GetPrices(
        [FromRoute] int id,
        [FromQuery] ListPriceQuery query,
        CancellationToken cancellationToken)
    {
        var prices = await _priceService.ListAsync(id, query, cancellationToken);
        return Ok(prices);
    }

    /// <summary>
    ///     Добавить цену
    /// </summary>
    [HttpPost("{id}/prices")]
    public async Task<IActionResult> AddPrice(
        [FromRoute] int id,
        [FromBody] AddPriceRequest request,
        CancellationToken cancellationToken)
    {
        request.BeerId = id;
        var prices = await _priceService.CreateAsync(request, cancellationToken);
        return Ok(prices);
    }

    /// <summary>
    ///     Получить источники
    /// </summary>
    [HttpGet("{id}/sources")]
    public async Task<IActionResult> GetSources(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var sources = await _sourceService.ListAsync(id, cancellationToken);
        return Ok(sources);
    }
}