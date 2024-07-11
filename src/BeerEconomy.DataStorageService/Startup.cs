using BeerEconomy.Common;
using BeerEconomy.Common.Helpers;
using BeerEconomy.DataStorageService.Database;
using BeerEconomy.DataStorageService.Database.Repositories.Impl;
using BeerEconomy.DataStorageService.Services;
using BeerEconomy.DataStorageService.Services.Impl;
using Microsoft.EntityFrameworkCore;

namespace BeerEconomy.DataStorageService;

internal sealed class Startup(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        var connectionString = Environment.GetEnvironmentVariable(Configs.CONNECTION_STRING)!;
        services.AddDbContext<DataContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString(connectionString)));

        services.AddScoped<BeerRepository>();
        services.AddScoped<PriceRepository>();
        services.AddScoped<SourceRepository>();
        services.AddScoped<IPriceService, PriceService>();
        services.AddScoped<ISourceService, SourceService>();
        services.AddScoped<IBeerService, BeerService>();
        services.ConfigureSwagger();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseSwaggerConfig();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}