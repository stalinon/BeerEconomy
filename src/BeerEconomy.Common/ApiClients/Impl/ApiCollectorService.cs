namespace BeerEconomy.Common.ApiClients.Impl;

internal sealed class ApiCollectorService : ApiClientBase
{
    public ApiCollectorService() : base(Environment.GetEnvironmentVariable(Configs.DATA_SERVICE_URL))
    {
    }
}