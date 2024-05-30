using Microsoft.Extensions.Logging;

using ParkingBot.Background;
using ParkingBot.Models.Parking;
using ParkingBot.Properties;
using ParkingBot.Services;
using ParkingBot.Util;

using Shiny;
using Shiny.BluetoothLE;
using Shiny.Jobs;
using Shiny.Locations;
using Shiny.Notifications;

namespace ParkingBot.Handlers;

public class MultiDelegate(ILogger<MultiDelegate> _logger, IJobManager _jobs,
        //KioskParkingService _kiosk,
        VehicleBluetoothService _bt,
        ServiceData _data)
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
