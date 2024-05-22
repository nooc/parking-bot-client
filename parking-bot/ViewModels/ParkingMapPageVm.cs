using Mapsui.Layers;
using Mapsui.Styles;

using Microsoft.Extensions.Logging;

namespace ParkingBot.ViewModels;

public class ParkingMapPageVm : BaseVm
{
    private readonly ILogger<MainPageVm> _logger;
    private string _Zoom = string.Empty;
    private Mapsui.Map? _Map;

    public string Zoom { get => _Zoom; set => SetProperty(ref _Zoom, value); }
    public ObservableMemoryLayer<Mapsui.UI.Maui.Pin> PinLayer { get; private set; }
    public Mapsui.Map? Map
    {
        get => _Map;
        internal set
        {
            _Map = value;
            if (_Map != null)
            {
                _Map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer(Properties.Values.USER_AGENT));
                //MapCtrl.Zoomed += MapCtrl_Zoomed;
                Zoom = $"{_Map.Navigator?.Viewport.Resolution}";
                LocationLayer = new MyLocationLayer(_Map);
                _Map.Layers.Add(PinLayer);
                _Map.Layers.Add(LocationLayer);
                UpdateLocation();
            }
        }
    }
    public MyLocationLayer? LocationLayer { get; internal set; }

    public ParkingMapPageVm(ILogger<MainPageVm> logger) : base()
    {
        _logger = logger;
        PinLayer = new(p => p.Feature)
        {
            Style = SymbolStyles.CreatePinStyle(symbolScale: 0.7)
        };

        //TODO: remove hard coded
        PinLayer.ObservableCollection = new System.Collections.ObjectModel.ObservableCollection<Mapsui.UI.Maui.Pin>
        {
            new() {
                Position=new Mapsui.UI.Maui.Position(57.73637027332549, 12.031604859973248),
                Label = "Hello",
                Type = Mapsui.UI.Maui.PinType.Pin,
                IsVisible=true,
            }
        };
    }
    protected override void ExecuteLoadModelCommand()
    {
        IsBusy = true;
        try
        {
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, nameof(ParkingMapPageVm));
        }
        finally
        {
            IsBusy = false;
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
                    _Map?.Navigator.CenterOnAndZoomTo(new Mapsui.MPoint(x, y), 2);
                }
                //UpdateLocation();
            });
    }
}
