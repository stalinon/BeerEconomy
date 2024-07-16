namespace BeerEconomy.Common.ApiClients;

/// <summary>
///     Сервис парсинга цен
/// </summary>
public interface ICollectorService
{
    /// <summary>
    ///     Запустить парс цен
    /// </summary>
    Task StartAsync(CancellationToken cancellationToken);
}