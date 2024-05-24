using ParkingBot.Models.Parking;

using Shiny;
using Shiny.Locations;

namespace ParkingBot.Services;

public class GeoFencingService(IGeofenceManager _geo, ServiceHelperService _helper)
{
    public record KioskRegion(KioskSite Site, string Identifier, Position Center, Distance Radius)
        : GeofenceRegion(Identifier, Center, Radius, false, true, true)
    {
        public GeofenceState State = GeofenceState.Unknown;
    }

    public bool IsGeoFencing => _geo.GetMonitorRegions().Count != 0;


    public async Task<bool> SetEnabled(bool enable)
    {
        if (enable)
        {
            List<Task> startTasks = [];
            _helper.GetRegions().ForEach(r => startTasks.Add(_geo.StartMonitoring(r)));
            await Task.WhenAll(startTasks);
            return true;
        }
        else
        {
            await _helper.StopAll();
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
