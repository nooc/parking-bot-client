using Mapsui;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;

using ParkingBot.Models.Parking;
using ParkingBot.Services;

namespace ParkingBot.Map;


public class PinProvider(GothenburgOpenDataService _service) : IProvider
{
    private readonly GothenburgOpenDataService dataService = _service;
    private static readonly string WGS84 = "EPSG:4326";
    private readonly List<IFeature> _Features = [];
    private MRect? _Rect = null;
    public IEnumerable<IFeature> Features => _Features;

    public string? CRS { get => WGS84; set => throw new InvalidOperationException(); }

    public MRect? GetExtent()
    {
        var extents = _Features.Where(f => f.Extent != null).Select(f => f.Extent ?? throw new ArgumentNullException());
        if (extents.Count() == 0) _Rect = null;
        else _Rect = new MRect(extents);

        return _Rect;
    }

    public async Task<IEnumerable<IFeature>> GetFeaturesAsync(FetchInfo fetchInfo)
    {
        /*
        var features = _Features
            .Where(f => f.Extent != null && fetchInfo.Extent.Contains(f.Extent));
        */

        if (_Features.Count == 0)
        {

            var pins = await dataService.GetNearestSiteInfosAsync(57.73645491460791, 12.031476445844772, 500);
            TollSiteInfo pin = pins.FirstOrDefault() ?? new();
            _Features.Add(
                new ParkingFeature(pin.Lat, pin.Long)
                {
                    Site = pin,
                    Styles = [
                        SymbolStyles.CreatePinStyle(Mapsui.Styles.Color.LightSkyBlue, symbolScale: 1),
                        new LabelStyle {
                            Text = pin.Name,
                            ForeColor = Mapsui.Styles.Color.Black,
                            Offset = new Offset(0,0),
                            BackColor = new Mapsui.Styles.Brush(Mapsui.Styles.Color.White)
                        }
                    ],
                });
        }
        return _Features;
    }
}