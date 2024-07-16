using System.Net;

namespace BeerEconomy.DataStorageService;

/// <summary>
///     Точка входа
/// </summary>
internal sealed class Program
{
    /// <inheritdoc cref="Program"/>
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>().UseKestrel(options =>
                {
                    options.ListenAnyIP(5080);
                });
            });
}