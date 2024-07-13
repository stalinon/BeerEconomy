using BeerEconomy.Common.ApiClients;
using BeerEconomy.Common.Helpers;
using BeerEconomy.PriceCollectorService.Schedule;
using BeerEconomy.PriceCollectorService.Services;
using BeerEconomy.PriceCollectorService.Services.Impl;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace BeerEconomy.PriceCollectorService;

internal sealed class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.ConfigureSwagger();
        services.AddApiServices();
        services.AddScoped<IParsingService, WinlabParsingService>();
        services.AddScoped<ParsingService>();
        
        services.AddSingleton<IJobFactory, SingletonJobFactory>();
        services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

        services.AddScoped<ParsingJob>();
        services.AddSingleton(new JobSchedule(jobType: typeof(ParsingJob), cronExpression: "0 0 3 ? * * *"));

        services.AddHostedService<QuartzHostedService>();
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