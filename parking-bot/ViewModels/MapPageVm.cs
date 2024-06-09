using Mapsui.Layers;
using Mapsui.Styles;

using Microsoft.Extensions.Logging;

using ParkingBot.Map;
using ParkingBot.Pages;
using ParkingBot.Services;

using SM = Mapsui.Projections.SphericalMercator;

namespace ParkingBot.ViewModels;

public class MapPageVm(ILogger<MapPageVm> logger, GothenburgOpenDataService _open) : BaseVm(logger)
{
    private static readonly string SPHERICAL_MERCATOR = "EPSG:3857";
    private static readonly string WGS84 = "EPSG:4326";

    private long VpChangeTime = 0;
    private string _Footer = string.Empty;
    private Mapsui.Map? _Map = null;
    private Layer? ParkingLayer = null;
    private readonly List<ParkingFeature> _Features = [];

    public string Footer { get => _Footer; set => SetProperty(ref _Footer, value); }

    public MyLocationLayer? LocationLayer { get; internal set; }

    protected override void ExecuteLoadModelCommand(Page page)
    {
        if (_Map == null && page is MapPage mapPage)
        {
            _Map = new Mapsui.Map { CRS = SPHERICAL_MERCATOR };

            if (ParkingLayer == null)
            {
                ParkingLayer = CreateParkingLayer();
            }
            var layer = Mapsui.Tiling.OpenStreetMap.CreateTileLayer($"{Properties.Values.USER_AGENT_NAME}/{Properties.Values.USER_AGENT_VER}");
            LocationLayer = new MyLocationLayer(_Map);

            _Map.CRS = SPHERICAL_MERCATOR;
            _Map.Navigator.ViewportChanged += Navigator_ViewportChanged;
            _Map.Navigator.RotationLock = true;
            _Map.Layers.Add(layer);
            _Map.Layers.Add(ParkingLayer);
            _Map.Layers.Add(LocationLayer);
            _Map.Info += MapInfo;
            _Footer = $"{_Map.Navigator?.Viewport.Resolution}";

            mapPage.SetMap(_Map);
            UpdateLocationInitial();
        }
    }

    public Layer CreateParkingLayer()
    {
        return new Layer
        {
            DataSource = new ProjectingPinProvider(new PinProvider(_open)),
            Name = "Parking",
            Style = null,
            IsMapInfoLayer = true
        };
    }

    //TODO cache parkings. 
    void TryLoadPins()
    {
        if (VpChangeTime < DateTime.Now.Ticks)
        {
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

    static IStyle _style = SymbolStyles.CreatePinStyle(Mapsui.Styles.Color.Red);
    private void MapInfo(object? sender, Mapsui.MapInfoEventArgs e)
    {

        if (e.MapInfo?.Feature is ParkingFeature feature)
        {
            feature.Styles.Clear();
            feature.Styles.Add(_style);
        }
    }
}
