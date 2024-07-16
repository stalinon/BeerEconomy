using BeerEconomy.AdminService.Authorization;
using BeerEconomy.AdminService.Components;
using BeerEconomy.Common.ApiClients;
using BeerEconomy.Common.Helpers.Exceptions;
using BeerEconomy.Common.Helpers.Logging;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;

namespace BeerEconomy.AdminService;

/// <summary>
///     Точка входа
/// </summary>
internal sealed class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorComponents().AddInteractiveServerComponents();
        services.AddRadzenComponents();
        services.AddApiServices();
        services.AddCustomLogging();
        
        services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseCustomLogging();
        app.UseMiddleware<ExceptionMiddleware>();

        app.UseRouting();
        app.UseAntiforgery();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorComponents<App>().AddInteractiveServerRenderMode();
        });
    }
}
