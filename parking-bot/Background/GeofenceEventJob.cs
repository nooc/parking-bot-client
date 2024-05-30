using Microsoft.Extensions.Logging;

using ParkingBot.Models.Parking;
using ParkingBot.Services;

using Shiny;
using Shiny.Jobs;
using Shiny.Locations;

namespace ParkingBot.Background;

internal class GeofenceEventJob(ILogger<DeviceEventJob> logger, IServiceProvider services) : Job(logger)
{
    private readonly Lazy<GeoFencingService> _geo = services.GetLazyService<GeoFencingService>();
    private readonly Lazy<TollParkingService> _toll = services.GetLazyService<TollParkingService>();
    private readonly Lazy<IGpsManager> _gps = services.GetLazyService<IGpsManager>();
    private readonly Lazy<IGeofenceManager> _geoMgr = services.GetLazyService<IGeofenceManager>();
    private readonly Lazy<ServiceData> _data = services.GetLazyService<ServiceData>();

    protected override async Task Run(CancellationToken cancelToken)
    {
        var args = JobInfo.Parameters ?? [];
        var identifier = args["identifier"];
        var site = _data.Value.ParkingSites[identifier];

        if (site.Intersecting)
        {
            site.Intersecting = true;
            // Start listening if not already.
            if (_gps.Value.CurrentListener == null)
            {
                await _gps.Value.StartListener(GpsRequest.Realtime(true));
            }
        }
        else
        {
            site.Intersecting = false;
            // remove parking if parked
            if (site.Parked)
            {
                _toll.Value.StopParking(site);
            }

            // Do not stop listener if there are other active (entered) regions.
            foreach (var region in _geoMgr.Value.GetMonitorRegions())
            {
                if (region is ParkingSite pRegion && pRegion.Intersecting)
                {
                    return;
                }
            }
            await _gps.Value.StopListener();
        }
    }
}
