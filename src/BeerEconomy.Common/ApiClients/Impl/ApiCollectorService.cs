namespace BeerEconomy.Common.ApiClients.Impl;

/// <inheritdoc cref="ICollectorService"/>
internal sealed class ApiCollectorService : ApiClientBase, ICollectorService
{
    /// <inheritdoc cref="ApiCollectorService"/>
    public ApiCollectorService() : base(Environment.GetEnvironmentVariable(Configs.COLLECTOR_SERVICE_URL)!)
    {
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var url = $"api/parse";
        await PostAsync(url, cancellationToken);
    }
}