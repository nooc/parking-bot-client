using ParkingBot.Models.Parking;

using Shiny.Locations;

namespace ParkingBot.Services;

public class GeoFencingService(IGeofenceManager _geo, ServiceHelperService _helper, ServiceData _data)
{
    public bool IsGeoFencing => _geo.GetMonitorRegions().Count != 0;

    public async Task<bool> SetEnabled(bool enable)
    {
        if (enable)
        {
            List<Task> startTasks = [];
            foreach (var (_, region) in _data.ParkingSites)
            {
                startTasks.Add(_geo.StartMonitoring(region));
            }
            await Task.WhenAll(startTasks);
            return true;
        }
        else
        {
            await _helper.StopAll();
            return true;
        }
    }

    public async Task<ParkingSite?> GetActiveRegion()
    {
        var loc = await Geolocation.Default.GetLastKnownLocationAsync();
        var pos = new Position(loc?.Latitude ?? 0, loc?.Longitude ?? 0);
        foreach (var region in _geo.GetMonitorRegions())
        {
            if (region.IsPositionInside(pos) && region is ParkingSite ps)
            {
                return ps;
            }
        }
        return null;
    }
}
