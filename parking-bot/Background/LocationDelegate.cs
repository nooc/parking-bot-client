using Microsoft.Extensions.Logging;

using ParkingBot.Background;
using ParkingBot.Models.Parking;
using ParkingBot.Properties;
using ParkingBot.Services;
using ParkingBot.Util;

using Shiny.Jobs;
using Shiny.Locations;

namespace ParkingBot.Handlers;

public class LocationDelegate(ILogger<LocationDelegate> _logger, IJobManager _jobs,
        //KioskParkingService _kiosk,
        VehicleBluetoothService _bt,
        ServiceData _data)
    : IGeofenceDelegate, IGpsDelegate
{
    //
    // IGpsDelegate
    //

    /// <summary>
    /// Gps handler
    /// </summary>
    /// <param name="reading"></param>
    /// <returns></returns>
    public Task OnReading(GpsReading reading)
    {
        _logger.LogInformation("On gps reading");
        // must have connected car
        var car = _bt.ConnectedCar;
        if (car != null && reading.PositionAccuracy <= Values.GPS_MIN_ACCURACY)
        {
            foreach (var (k, v) in _data.ParkingSites)
            {
                // check parking area occupancy

                if (v.Parked && !GeoTools.Intersect(reading.Position, reading.PositionAccuracy, v))
                {
                    // parked and not intersecting
                    Dictionary<string, string> jobParams = new()
                    {
                        { "site", v.Identifier },
                        { "car", car.RegNumber },
                        { "action", "stop" }
                    };
                    _jobs.Register(new JobInfo($"{nameof(ParkingJob)}_stop", typeof(ParkingJob), false, jobParams));
                }
                else if (!v.Parked && GeoTools.Intersect(reading.Position, reading.PositionAccuracy, v))
                {
                    // not parked and intersecting
                    Dictionary<string, string> jobParams = new()
                    {
                        { "site", v.Identifier },
                        { "car", car.RegNumber },
                        { "action", "start" }
                    };
                    _jobs.Register(new JobInfo($"{nameof(ParkingJob)}_start", typeof(ParkingJob), false, jobParams));
                }
            }
        }
        return Task.CompletedTask;
    }

    //
    // IGeofenceDelegate
    //

    // Region intersection event handler for when parking engine is active.
    public Task OnStatusChanged(GeofenceState newStatus, GeofenceRegion region)
    {
        Dictionary<string, string> jobParams = new()
        {
            { "identifier", region.Identifier },
        };

        if (region is ParkingSite site)
        {
            if (newStatus == GeofenceState.Entered)
            {
                site.Intersecting = true;
            }
            else if (newStatus == GeofenceState.Exited)
            {
                site.Intersecting = false;
            }
            else return Task.CompletedTask;

            _jobs.Register(new JobInfo(nameof(GeofenceEventJob), typeof(GeofenceEventJob), false, jobParams));
        }
        return Task.CompletedTask;
    }
}
