using Microsoft.Extensions.Logging;

using ParkingBot.Background;
using ParkingBot.Factories;
using ParkingBot.Models.Parking;
using ParkingBot.Properties;
using ParkingBot.Services;
using ParkingBot.Util;

using Shiny.Jobs;
using Shiny.Locations;
using Shiny.Notifications;

using System.Text.Json;

namespace ParkingBot.Handlers;

public class MultiDelegate : IGeofenceDelegate, IGpsDelegate, INotificationDelegate
{
    private readonly IGpsManager _gps;
    private readonly IGeofenceManager _geo;
    private readonly IJobManager _jobs;
    private readonly ParkingSettings _settings;
    private readonly KioskParkingService _kiosk;
    private readonly TollParkingService _sms;
    private readonly ILogger _logger;

    public MultiDelegate(ILogger<MultiDelegate> logger, IGpsManager gps, IGeofenceManager geo, IJobManager jobs,
        ParkingSettingsFactoryService parkingSettings, KioskParkingService kiosk, TollParkingService sms)
    {
        _logger = logger;
        _gps = gps;
        _geo = geo;
        _jobs = jobs;
        _settings = parkingSettings.Instance;
        _kiosk = kiosk;
        _sms = sms;
    }

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

        if (reading.PositionAccuracy <= _settings.MinGpsAccuracy)
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
                        if (dist.TotalMeters <= _settings.MaxGpsDistance)
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
}
