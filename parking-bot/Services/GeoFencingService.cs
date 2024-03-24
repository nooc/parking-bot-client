using ParkingBot.Factories;
using ParkingBot.Models.Parking;
using Shiny;
using Shiny.Locations;

namespace ParkingBot.Services;

public class GeoFencingService
{
    public record KioskRegion(KioskSite Site, string Identifier, Position Center, Distance Radius)
        : GeofenceRegion(Identifier, Center, Radius, false, true, true)
    {
        public GeofenceState State = GeofenceState.Unknown;
    }

    private readonly ServiceHelperService _access;
    private readonly IGeofenceManager _geo;
    private readonly ParkingSettings _parkingSettings;
    private readonly List<GeofenceRegion> _regions = [];
    public bool IsGeoFencing => _geo.GetMonitorRegions().Count != 0;

    public GeoFencingService(IGeofenceManager geofencer, ServiceHelperService access, ParkingSettingsFactoryService parkingSettings)
    {
        _access = access;
        _geo = geofencer;
        _parkingSettings = parkingSettings.Instance;
        if (_parkingSettings.Kiosk != null && _parkingSettings.Kiosk.Sites != null)
        {
            foreach (var site in _parkingSettings.Kiosk.Sites)
            {
                if (site.SiteId != 0 && site.Location != null)
                {
                    _regions.Add(new KioskRegion(site, site.SiteId.ToString(), new(site.Location.Lat, site.Location.Lon), Distance.FromMeters(_parkingSettings.RegionRadius)));
                }
            }
        }
    }

    public async Task<bool> SetEnabled(bool enable)
    {
        if (enable)
        {
            var hasAccess = await _access.RequestAccess();
            var hasPrereq = await _access.HasSettingsPrerequisites();
            if (!(hasPrereq && hasAccess)) return false;

            List<Task> startTasks = [];
            _regions.ForEach(r => startTasks.Add(_geo.StartMonitoring(r)));
            await Task.WhenAll(startTasks);
            return true;
        }
        else
        {
            await _access.StopAll();
            return true;
        }
    }

    public async Task<KioskSite?> GetActiveRegion()
    {
        var loc = await Geolocation.Default.GetLastKnownLocationAsync();
        var pos = new Position(loc?.Latitude ?? 0, loc?.Longitude ?? 0);
        foreach (var region in _geo.GetMonitorRegions())
        {
            if (region.IsPositionInside(pos) && region is KioskRegion r)
            {
                return r.Site;
            }
        }
        return null;
    }
}
