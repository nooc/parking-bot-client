using Microsoft.Extensions.Logging;

using ParkingBot.Background;
using ParkingBot.Properties;
using ParkingBot.Services;
using ParkingBot.Util;

using Shiny;
using Shiny.BluetoothLE;
using Shiny.Jobs;
using Shiny.Locations;
using Shiny.Notifications;

using System.Text.Json;

namespace ParkingBot.Handlers;

public class MultiDelegate(ILogger<MultiDelegate> _logger, IGpsManager _gps, IGeofenceManager _geo, IJobManager _jobs,
        KioskParkingService _kiosk, TollParkingService _sms, VehicleBluetoothService _vbt)
    : IBleDelegate, IGeofenceDelegate, IGpsDelegate, INotificationDelegate
{
    // NOTIFICATIONS
    public Task OnEntry(NotificationResponse response)
    {
        return Task.CompletedTask;
    }

    // GPS
    // Gps update event handler for when inside a region.
    public Task OnReading(GpsReading reading)
    {
        _logger.LogInformation("On gps reading");

        if (reading.PositionAccuracy <= Values.GPS_MIN_ACCURACY)
        {
            // check that no ongoing parking exists
            if (_kiosk.OngoingParking == null && !_jobs.IsRunning)
            {
                var today = DateTime.Now.DayOfWeek;
                foreach (var region in _geo.GetMonitorRegions())
                {
                    if (region is GeoFencingService.KioskRegion kregion && kregion.State == GeofenceState.Entered)
                    {
                        // get valid days
                        var days = kregion.Site.Days?.Select(DateTimeUtils.FromStringDay) ?? Enumerable.Empty<DayOfWeek>();
                        // get distance to region center
                        var dist = reading.Position.GetDistanceTo(region.Center);
                        // chech distance
                        if (dist.TotalMeters <= Values.GPS_MAX_DISTANCE)
                        {
                            // do parking
                            Dictionary<string, string> jobParams = new()
                            {
                                { "has_kiosk", days.Contains(DateTime.Now.DayOfWeek).ToString() },
                                { "site", JsonSerializer.Serialize(kregion.Site) }
                            };
                            _jobs.Register(new JobInfo(Values.PARKING_JOB, typeof(ParkingJob), false, jobParams));
                        }
                    }
                }
            }
        }
        return Task.CompletedTask;
    }

    // GEO FENCING
    // Region intersection event handler for whe parking engine is active.
    public async Task OnStatusChanged(GeofenceState newStatus, GeofenceRegion region)
    {
        if (region is GeoFencingService.KioskRegion kregion) kregion.State = newStatus;

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
            if (_sms.OngoingParking != null)
            {
                _sms.StopParking();
            }

            // Do not stop listener if there are other active (entered) regions.
            foreach (var r in _geo.GetMonitorRegions())
            {
                if (r is GeoFencingService.KioskRegion _region && _region.State == GeofenceState.Entered)
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
            _vbt.Connect(peripheral.Uuid, peripheral.Name);
        }
        else if (peripheral.Status == ConnectionState.Disconnected)
        {
            _vbt.Disconnect(peripheral.Uuid);
        }

        return Task.CompletedTask;
    }

    public Task OnAdapterStateChanged(AccessState state)
    {
        return Task.CompletedTask;
    }
}
