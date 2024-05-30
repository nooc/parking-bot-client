using Microsoft.Extensions.Logging;

using ParkingBot.Background;
using ParkingBot.Models.Parking;
using ParkingBot.Properties;
using ParkingBot.Services;

using Shiny;
using Shiny.BluetoothLE;
using Shiny.Jobs;
using Shiny.Locations;
using Shiny.Notifications;

using System.Text.Json;

namespace ParkingBot.Handlers;

public class MultiDelegate(ILogger<MultiDelegate> _logger, IGpsManager _gps, IGeofenceManager _geo, IJobManager _jobs,
        //KioskParkingService _kiosk,
        VehicleBluetoothService _bt,
        TollParkingService _toll, ServiceHelperService _hlp)
    : IBleDelegate, IGeofenceDelegate, IGpsDelegate, INotificationDelegate
{
    // NOTIFICATIONS
    public Task OnEntry(NotificationResponse response)
    {
        return Task.CompletedTask;
    }

    // GPS
    // Gps update event handler for when inside a region.
    public async Task OnReading(GpsReading reading)
    {
        _logger.LogInformation("On gps reading");
        // must have connected car
        var car = _bt.ConnectedCar;
        if (car != null && reading.PositionAccuracy <= Values.GPS_MIN_ACCURACY)
        {
            // check that no ongoing parking exists
            if (_toll.OngoingParking == null && !_jobs.IsRunning)
            {
                var settings = await _hlp.GetSettings();
                if (settings?.User.Phone == null)
                {
                    _logger.LogError("User phone not set.");
                    return;
                }
                // for all regions
                var today = DateTime.Now.DayOfWeek;
                foreach (var region in _geo.GetMonitorRegions())
                {
                    // if inside region
                    if (region is GeoFencingService.SiteRegion sRegion && sRegion.State == GeofenceState.Entered)
                    {
                        //TODO: polygon check if poly data

                        // get distance to region center
                        var dist = reading.Position.GetDistanceTo(region.Center);
                        // check distance
                        if (dist.TotalMeters <= Values.GPS_MAX_DISTANCE)
                        {
                            // do parking
                            Dictionary<string, string> jobParams = new()
                            {
                                { "site", JsonSerializer.Serialize(sRegion.Site) },
                                { "site_type", "toll" },
                                { "car", JsonSerializer.Serialize(car) },
                                { "phone", settings.User.Phone },
                            };
                            _jobs.Register(new JobInfo(Values.PARKING_JOB, typeof(ParkingJob), false, jobParams));
                        }
                    }
                }
            }
        }
    }

    // GEO FENCING
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

    // BLUETOOTH
    // Parking actions should only occure while we have a device connection. 
    public Task OnPeripheralStateChanged(IPeripheral peripheral)
    {
        Dictionary<string, string> jobParams = new()
        {
            { "uuid", peripheral.Uuid },
            { "name", peripheral.Name??string.Empty }
        };

        if (peripheral.Status == ConnectionState.Connected)
        {
            jobParams.Add("state", "connected");
        }
        else if (peripheral.Status == ConnectionState.Disconnected)
        {
            jobParams.Add("state", "disconnected");
        }
        else return Task.CompletedTask;
        _jobs.Register(new JobInfo(nameof(DeviceEventJob), typeof(DeviceEventJob), false, jobParams));
        return Task.CompletedTask;
    }

    public Task OnAdapterStateChanged(AccessState state)
    {
        return Task.CompletedTask;
    }
}
