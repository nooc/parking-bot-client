using ParkingBot.ViewModels;

namespace ParkingBot.Pages;

public partial class ParkingMapPage : ContentPage
{
    private ParkingMapPageVm Vm { get { if (BindingContext is ParkingMapPageVm vm) return vm; else throw new ArgumentNullException("BindingContext"); } }
    public ParkingMapPage()
    {
        InitializeComponent();
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        if (MapCtrl.Map is Mapsui.Map map)
        {
            map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer(Properties.Values.USER_AGENT));
            map.Info += Map_MapInfo;
            //MapCtrl.Zoomed += MapCtrl_Zoomed;
            Vm.Zoom = $"{map.Navigator?.Viewport.Resolution}";
            UpdateLocation();
        }
    }

    private void UpdateLocation()
    {
        Geolocation.Default.GetLocationAsync()
            .ContinueWith(task =>
            {
                if (task.Result is Location loc)
                {
                    var (x, y) = Mapsui.Projections.SphericalMercator.FromLonLat(loc.Longitude, loc.Latitude);
                    MapCtrl.Map.Navigator.CenterOnAndZoomTo(new Mapsui.MPoint(x, y), 2);
                }
                UpdateLocation();
            });
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
            Vm.Zoom = $"{map.Navigator?.Viewport.Resolution}";
        }
        e.Handled = false;
    }

    private void Map_MapInfo(object? sender, Mapsui.MapInfoEventArgs e)
    {
    }


}