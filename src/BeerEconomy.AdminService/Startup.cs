using BeerEconomy.AdminService.Components;
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

        app.UseRouting();
        app.UseAntiforgery();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorComponents<App>().AddInteractiveServerRenderMode();
        });
    }
}
