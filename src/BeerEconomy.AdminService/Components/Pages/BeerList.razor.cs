using BeerEconomy.Common.ApiClients;
using BeerEconomy.Common.Models.Requests.Beers;
using BeerEconomy.Common.Models.Responses;
using BeerEconomy.Common.Models.Responses.Beers;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace BeerEconomy.AdminService.Components.Pages;

/// <summary>
///     Страница для отображения имеющихся в системе пив
/// </summary>
public partial class BeerList
{
    /// <summary>
    ///     Сервис пив
    /// </summary>
    [Inject]
    public IBeerService BeerService { get; set; } = default!;
    
    private const string PagingSummaryFormat = "Страница {0} из {1} <b>(всего {2} записей)</b>";
    private const string PageTitle = "Пивы";
    private const int PageSize = 20;
    RadzenDataGrid<BeerModel> _dataGrid = default!;
    
    private PagedList<BeerModel> _models = new();

    protected override async Task OnInitializedAsync()
    {
        var query = new ListBeerQuery()
        {
            Max = PageSize,
            Skip = 0
        };

        _models = await BeerService.ListAsync(query, CancellationToken.None);
    }
    
    private async Task OnPage(PagerEventArgs args)
    {
        var query = new ListBeerQuery()
        {
            Max = args.Top,
            Skip = args.Skip
        };

        _models = await BeerService.ListAsync(query, CancellationToken.None);
    }

    private async Task DeleteAsync(int id)
    {
        await BeerService.DeleteAsync(id, CancellationToken.None);
    }
}