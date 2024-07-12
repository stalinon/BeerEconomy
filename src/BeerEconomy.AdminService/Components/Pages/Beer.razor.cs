using BeerEconomy.Common.ApiClients;
using BeerEconomy.Common.Models.Requests.Beers;
using BeerEconomy.Common.Models.Requests.Sources;
using BeerEconomy.Common.Models.Responses.Beers;
using BeerEconomy.Common.Models.Responses.Sources;
using Microsoft.AspNetCore.Components;

namespace BeerEconomy.AdminService.Components.Pages;

/// <summary>
///     Старница одиночного пива
/// </summary>
public partial class Beer
{
    /// <summary>
    ///     Идентификатор пива
    /// </summary>
    [Parameter]
    public string Id { get; set; } = default!;
    
    /// <summary>
    ///     Сервис пив
    /// </summary>
    [Inject]
    public IBeerService BeerService { get; set; } = default!;
    
    /// <summary>
    ///     Сервис источников
    /// </summary>
    [Inject]
    public ISourceService SourceService { get; set; } = default!;

    private string PageTitle => Id == "0" ? "Создание пива" : "Пиво";

    private BeerModel _beerModel = new();
    private List<SourceModel> _sources = new();
    private readonly List<SourceModel> _sourcesToDelete = new();

    protected override async Task OnInitializedAsync()
    {
        if (!int.TryParse(Id, out var id))
        {
            throw new Exception();
        }
        
        if (id != default)
        {
            _beerModel = await BeerService.GetAsync(id, CancellationToken.None);
            _sources = await SourceService.ListAsync(id, CancellationToken.None);
        }
    }

    private void AddSource()
    {
        _sources.Add(new());
        StateHasChanged();
    }

    private void DeleteSource(SourceModel source)
    {
        _sources.Remove(source);
        _sourcesToDelete.Add(source);
        StateHasChanged();
    }

    private async Task SaveAsync()
    {
        if (!int.TryParse(Id, out var id))
        {
            throw new Exception();
        }

        await HandleBeersAsync(id);
        await HandleSourcesAsync();
    }

    private async Task HandleBeersAsync(int id)
    {
        if (id == default)
        {
            var addBeerRequest = new AddBeerRequest
            {
                Name = _beerModel.Name,
                Description = _beerModel.Description,
                ImageUrl = _beerModel.ImageUrl
            };

            _beerModel = await BeerService.CreateAsync(addBeerRequest, CancellationToken.None);
        }
        else
        {
            var updateBeerRequest = new UpdateBeerRequest
            {
                Name = _beerModel.Name,
                Description = _beerModel.Description,
                ImageUrl = _beerModel.ImageUrl
            };

            _beerModel = await BeerService.UpdateAsync(_beerModel.Id, updateBeerRequest, CancellationToken.None);
        }
    }

    private async Task HandleSourcesAsync()
    {
        foreach (var source in _sources)
        {
            if (source.Id == default)
            {
                var addSourceRequest = new AddSourceRequest
                {
                    BeerId = _beerModel.Id,
                    Source = source.Source,
                    Url = source.Url
                };

                await SourceService.CreateAsync(addSourceRequest, CancellationToken.None);
            }
            else
            {
                var addSourceRequest = new UpdateSourceRequest
                {
                    BeerId = _beerModel.Id,
                    Url = source.Url
                };

                await SourceService.UpdateAsync(source.Id, addSourceRequest, CancellationToken.None);
            }
        }

        foreach (var source in _sourcesToDelete.Where(s => s.Id != default))
        {
            await SourceService.DeleteAsync(source.Id, CancellationToken.None);
        }
    }
}