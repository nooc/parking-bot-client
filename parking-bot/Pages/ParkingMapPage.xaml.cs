using ParkingBot.ViewModels;

namespace ParkingBot.Pages;

public partial class ParkingMapPage : ContentPage
{
    private readonly ParkingMapPageVm Vm;

    public ParkingMapPage(ParkingMapPageVm viewModel)
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

    private void MapInfo(object? sender, Mapsui.MapInfoEventArgs e)
    {
    }
}