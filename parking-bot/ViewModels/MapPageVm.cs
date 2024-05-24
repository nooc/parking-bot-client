using Mapsui;
using Mapsui.Layers;
using Mapsui.Styles;

using Microsoft.Extensions.Logging;

namespace ParkingBot.ViewModels;

public class MapPageVm : BaseVm
{
    private string _Zoom = string.Empty;
    private Mapsui.Map? _Map;

    public string Zoom { get => _Zoom; set => SetProperty(ref _Zoom, value); }
    public MemoryLayer FeatureLayer { get; private set; }
    public Mapsui.Map? Map
    {
        get => _Map;
        internal set
        {
            _Map = value;
            if (_Map != null)
            {
                //_Map.Navigator.ViewChanged += Map_ViewChanged;
                _Map.Navigator.RotationLock = true;
                _Map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer(Properties.Values.USER_AGENT));
                Zoom = $"{_Map.Navigator?.Viewport.Resolution}";
                LocationLayer = new MyLocationLayer(_Map);
                _Map.Layers.Add(FeatureLayer);
                _Map.Layers.Add(LocationLayer);
                UpdateLocation();
            }
        }
    }
    public MyLocationLayer? LocationLayer { get; internal set; }

    public MapPageVm(ILogger<MapPageVm> logger) : base(logger)
    {
        var (x, y) = Mapsui.Projections.SphericalMercator.FromLonLat(12.031604859973248, 57.73637027332549);
        IFeature[] features = [
            new PointFeature(x,y)
            {
                Styles = [SymbolStyles.CreatePinStyle(symbolScale: 0.7)],

            },
        ];
        FeatureLayer = new MemoryLayer
        {
            Features = features,
            Name = "Demo",
            Style = null,
        };
    }
    protected override void ExecuteLoadModelCommand()
    {
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

    /// <summary>
    /// Enable rendering of geometry based on zoom level.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    internal void MapCtrl_Zoomed(object? sender, Mapsui.UI.ZoomedEventArgs e)
    {
        Zoom = $"{_Map?.Navigator?.Viewport.Resolution}";
        e.Handled = false;
    }
}
