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

    /// <summary>
    /// Limit rendering of geometry based on zoom level
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void MapCtrl_Zoomed(object? sender, Mapsui.UI.ZoomedEventArgs e)
    {
        if (MapCtrl.Map is Mapsui.Map map)
        {

        }
        e.Handled = false;
    }

    private void MapInfo(object? sender, Mapsui.MapInfoEventArgs e)
    {
    }
}