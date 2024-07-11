using BeerEconomy.Common.Models.Requests.Sources;
using BeerEconomy.DataStorageService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BeerEconomy.DataStorageService.Controllers;

/// <summary>
///     Контроллер источников
/// </summary>
[Route("~/api/sources")]
public sealed class SourceController : Controller
{
    private readonly ISourceService _sourceService;

    /// <inheritdoc href="SourceController" />
    public SourceController(ISourceService sourceService)
    {
        _sourceService = sourceService;
    }

    /// <summary>
    ///     Создать источник
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] AddSourceRequest request,
        CancellationToken cancellationToken)
    {
        var source = await _sourceService.CreateAsync(request, cancellationToken);
        return Ok(source);
    }

    /// <summary>
    ///     Обновить источник
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        [FromRoute] int id,
        [FromBody] UpdateSourceRequest request,
        CancellationToken cancellationToken)
    {
        var source = await _sourceService.UpdateAsync(id, request, cancellationToken);
        return Ok(source);
    }

    /// <summary>
    ///     Удалить источник
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await _sourceService.DeleteAsync(id, cancellationToken);
        return Ok();
    }
}