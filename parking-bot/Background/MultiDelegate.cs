using Microsoft.Extensions.Logging;

using ParkingBot.Background;
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
    // Region intersection event handler for whe parking engine is active.
    public async Task OnStatusChanged(GeofenceState newStatus, GeofenceRegion region)
    {
        if (region is GeoFencingService.SiteRegion kregion) kregion.State = newStatus;

        if (newStatus == GeofenceState.Entered)
        {
            // Start listening if not already.
            if (_gps.CurrentListener == null)
            {
                await _gps.StartListener(GpsRequest.Realtime(true));
            }
        }
        else if (newStatus == GeofenceState.Exited)
        {
            // remove parking
            if (_toll.OngoingParking != null)
            {
                _toll.StopParking();
            }

            // Do not stop listener if there are other active (entered) regions.
            foreach (var r in _geo.GetMonitorRegions())
            {
                if (r is GeoFencingService.SiteRegion _region && _region.State == GeofenceState.Entered)
                {
                    return;
                }
            }
            await _gps.StopListener();
        }
    }

    // BLUETOOTH
    // Parking actions should only occure while we have a device connection. 
    public Task OnPeripheralStateChanged(IPeripheral peripheral)
    {
        if (peripheral.Status == ConnectionState.Connected)
        {
            _bt.Connect(peripheral.Uuid, peripheral.Name);
        }
        else if (peripheral.Status == ConnectionState.Disconnected)
        {
            _bt.Disconnect(peripheral.Uuid);
        }

        return Task.CompletedTask;
    }

    public Task OnAdapterStateChanged(AccessState state)
    {
        return Task.CompletedTask;
    }
}
