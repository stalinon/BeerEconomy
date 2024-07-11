using BeerEconomy.Common.ApiClients;
using BeerEconomy.Common.Helpers;
using BeerEconomy.DataStorageService.Database;
using BeerEconomy.DataStorageService.Database.Repositories.Impl;
using BeerEconomy.DataStorageService.Services;
using BeerEconomy.DataStorageService.Services.Impl;
using Microsoft.EntityFrameworkCore;

namespace BeerEconomy.DataStorageService;

internal sealed class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddDbContext<DataContext>();

        services.AddScoped<BeerRepository>();
        services.AddScoped<PriceRepository>();
        services.AddScoped<SourceRepository>();
        services.AddScoped<IPriceService, PriceService>();
        services.AddScoped<ISourceService, SourceService>();
        services.AddScoped<IBeerService, BeerService>();
        services.ConfigureSwagger();

        ApplyMigrations(services.BuildServiceProvider());
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
    
    private static void ApplyMigrations(IServiceProvider serviceProvider)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();

            db.Database.SetCommandTimeout(TimeSpan.FromDays(2));
            db.Database.Migrate();
            db.Database.SetCommandTimeout(null);
        }
        catch (Exception ex)
        {
            throw new Exception("Error applying database migrations", ex);
        }
    }
}