using BeerEconomy.Common.Models.Responses.Sources;
using BeerEconomy.PriceCollectorService.Services.Impl;
using Quartz;

namespace BeerEconomy.PriceCollectorService.Schedule;

/// <summary>
///     Задача расписания парсинга
/// </summary>
internal sealed class ParsingJob(ParsingService parsingService) : IJob
{
    /// <inheritdoc />
    public async Task Execute(IJobExecutionContext context)
    {
        var sources = new Dictionary<int, SourceModel>();
        var cancellationToken = context.CancellationToken;
        await parsingService.ParsePricesAsync(sources, cancellationToken);
    }
}