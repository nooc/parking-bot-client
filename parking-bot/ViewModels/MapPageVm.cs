using Mapsui.Layers;
using Mapsui.Styles;

using Microsoft.Extensions.Logging;

using ParkingBot.Services;

using SM = Mapsui.Projections.SphericalMercator;

namespace ParkingBot.ViewModels;

public class MapPageVm(ILogger<MapPageVm> logger, GothenburgOpenDataService _open) : BaseVm(logger)
{
    private long VpChangeTime = 0;
    private string _Footer = string.Empty;
    private Mapsui.Map? _Map;
    private MemoryLayer _FeatureLayer = new MemoryLayer
    {
        Name = "Parkings",
        Style = null,
    };
    public string Footer { get => _Footer; set => SetProperty(ref _Footer, value); }
    public MemoryLayer FeatureLayer => _FeatureLayer;

    public Mapsui.Map? Map
    {
        get => _Map;
        internal set
        {
            _Map = value;
            if (_Map != null)
            {
                _Map.Navigator.ViewportChanged += Navigator_ViewportChanged;
                _Map.Navigator.RotationLock = true;

                _Map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer($"{Properties.Values.USER_AGENT_NAME}/{Properties.Values.USER_AGENT_VER}"));
                _Footer = $"{_Map.Navigator?.Viewport.Resolution}";
                LocationLayer = new MyLocationLayer(_Map);
                _Map.Layers.Add(FeatureLayer);
                _Map.Layers.Add(LocationLayer);
                UpdateLocationInitial();
            }
        }
    }

    public MyLocationLayer? LocationLayer { get; internal set; }

    protected override void ExecuteLoadModelCommand()
    {
    }

    //TODO cache parkings. 
    async void TryLoadPins()
    {
        if (VpChangeTime < DateTime.Now.Ticks)
        {
            var (lon, lat) = SM.ToLonLat(_Map?.Navigator.Viewport.CenterX ?? 0, _Map?.Navigator.Viewport.CenterY ?? 0);
            var pins = await _open.GetNearestSiteInfosAsync(lat, lon, 500);
            var features = pins.Select(p =>
            {
                var (x, y) = SM.FromLonLat(p.Long, p.Lat);
                return new PointFeature(x, y)
                {
                    Styles = [SymbolStyles.CreatePinStyle(symbolScale: .8)]
                };
            });
            _FeatureLayer.Features = features;
            _Map?.Refresh(Mapsui.ChangeType.Discrete);
        }
        else
        {
            Dispatcher.GetForCurrentThread()?.DispatchDelayed(TimeSpan.FromSeconds(1), TryLoadPins);
        }
    }

    private void Navigator_ViewportChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var last = VpChangeTime;
        VpChangeTime = DateTime.Now.Ticks + TimeSpan.TicksPerSecond;
        if (last < DateTime.Now.Ticks)
        {
            // only if not already dispatched (last> now assume dispatched) 
            Dispatcher.GetForCurrentThread()?.DispatchDelayed(TimeSpan.FromSeconds(1), TryLoadPins);
        }
    }


    private void UpdateLocationInitial()
    {
        Geolocation.Default.GetLocationAsync()
            .ContinueWith(task =>
            {
                if (task.Result is Location loc)
                {
                    var (x, y) = SM.FromLonLat(loc.Longitude, loc.Latitude);
                    _Map?.Navigator.CenterOnAndZoomTo(new Mapsui.MPoint(x, y), 2);
                }
                //UpdateLocation();
            });
    }
    /*
    private static SymbolStyle CreateSvgStyle(string mapPin, double scale)
    {
        if (!BitmapRegistry.Instance.TryGetBitmapId(mapPin, out var bitmapId))
        {
            return new SymbolStyle { BitmapId = GetSvgId(mapPin), SymbolScale = scale, SymbolOffset = new RelativeOffset(0.0, 0.5) };
        }
        return
    }

    private int GetSvgId(string mapPin)
    {
        throw new NotImplementedException();
    }*/
    public void UpdateLocation(Location loc)
    {
        var (x, y) = SM.FromLonLat(loc.Longitude, loc.Latitude);
        LocationLayer?.UpdateMyLocation(new Mapsui.MPoint(x, y), true);
    }
}
