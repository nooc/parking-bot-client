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

        Appearing += MapPage_Appearing;
        Disappearing += MapPage_Disappearing;
    }

    private void MapPage_Disappearing(object? sender, EventArgs e)
    {
        _locationUpdates = false;
    }

    private void MapPage_Appearing(object? sender, EventArgs e)
    {
        _locationUpdates = true;
        Dispatcher.StartTimer(TimeSpan.FromSeconds(2), UpdateLocation);
        Vm.LoadModelCommand.Execute(this);
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

    internal void SetMap(Mapsui.Map map)
    {
        MapCtrl.Map = map;
    }
}