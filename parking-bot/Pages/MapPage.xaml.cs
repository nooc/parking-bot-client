using ParkingBot.ViewModels;

namespace ParkingBot.Pages;

public partial class MapPage : ContentPage
{
    private readonly MapPageVm Vm;
    private bool _locationUpdates = true;

    public MapPage(MapPageVm viewModel)
    {
        InitializeComponent();

        BindingContext = Vm = viewModel;
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        if (MapCtrl.Map is Mapsui.Map map)
        {
            Vm.Map = map;
            map.Info += MapInfo;
        }
    }

    private async void MapInfo(object? sender, Mapsui.MapInfoEventArgs e)
    {
        await DisplayAlert("info", e.MapInfo?.WorldPosition?.ToString(), "ok");
    }

    protected override void OnAppearing()
    {
        _locationUpdates = true;
        Dispatcher.StartTimer(TimeSpan.FromSeconds(4), UpdateLocation);
        Vm.LoadModelCommand.Execute(this);
    }
    protected override void OnDisappearing()
    {
        _locationUpdates = false;
    }

    private async void DoUpdateLocation()
    {
        var loc = await Geolocation.GetLocationAsync();
        if (loc != null)
        {
            Vm.UpdateLocation(loc);
        }
    }

    private bool UpdateLocation()
    {
        DoUpdateLocation();
        return _locationUpdates;
    }
}