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

        services.AddDbContext<DataContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<BeerRepository>();
        services.AddScoped<PriceRepository>();
        services.AddScoped<SourceRepository>();
        services.AddScoped<IPriceService, PriceService>();
        services.AddScoped<ISourceService, SourceService>();
        services.AddScoped<IBeerService, BeerService>();
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

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}