using ParkingBot.ViewModels;

namespace ParkingBot.Pages;

public partial class MapPage : ContentPage
{
    private readonly MapPageVm Vm;

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

    private void MapInfo(object? sender, Mapsui.MapInfoEventArgs e)
    {
    }
}