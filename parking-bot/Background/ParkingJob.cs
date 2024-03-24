using Microsoft.Extensions.Logging;

using ParkingBot.Models.Parking;
using ParkingBot.Properties;
using ParkingBot.Services;

using Shiny.Jobs;
using Shiny.Notifications;

using System.Text.Json;

namespace ParkingBot.Background;

public class ParkingJob : Job
{
    private readonly ILogger<ParkingJob> _logger;
    private readonly INotificationManager _notifications;
    private readonly KioskParkingService _kiosk;
    private readonly TollParkingService _sms;
    private readonly ServiceHelperService _helper;

    public ParkingJob(ILogger<ParkingJob> logger, INotificationManager notificationManager, KioskParkingService kioskParkingService,
        TollParkingService smsParkingService, ServiceHelperService helper) : base(logger)
    {
        _logger = logger;
        _notifications = notificationManager;
        _kiosk = kioskParkingService;
        _sms = smsParkingService;
        _helper = helper;
    }

    protected override async Task Run(CancellationToken cancelToken)
    {
        var hasPrereq = await _helper.HasSettingsPrerequisites();

        if (!hasPrereq)
        {
            await _helper.StopAll();
            await _notifications.Send(new Notification
            {
                Title = Lang.auto_parking,
                Message = Lang.settings_error
            });
            return;
        }

        if (JobInfo.Parameters?.TryGetValue("site", out var siteJson) ?? false)
        {
            string? hasKiosk = null;
            JobInfo.Parameters?.TryGetValue("has_kiosk", out hasKiosk);
            string plate = Preferences.Get(Values.PARKING_LICENSEPLATE_KEY, string.Empty).Trim();
            string phone = Preferences.Get(Values.PARKING_PHONE_KEY, string.Empty).Trim();
            bool reminder = Preferences.Get(Values.PARKING_REMINDER_KEY, false);
            bool useSmsParking = Preferences.Get(Values.PARKING_USE_SMS_PARKING_KEY, false);
            var site = JsonSerializer.Deserialize<KioskSite>(siteJson);
            if (site != null) TryPark(site, plate, reminder ? phone : null, hasKiosk?.Equals("True") ?? false, useSmsParking);
        }
    }

    private async void TryPark(KioskSite site, string plate, string? phone, bool hasKiosk, bool useSmsParking)
    {
        // try kiosk parking
        if (hasKiosk)
        {
            var result = await _kiosk.ParkAsync(new KioskParkingRequest
            {
                externalId = site.ExternalId ?? string.Empty,
                name = string.Empty,
                registrationNumber = plate,
                phoneNumber = phone ?? string.Empty,
                setEndTimeReminder = phone != null,
            }, site);

            if (result is KioskParkingTicket ticket)
            {
                _sms.StopParking();
                var dt = DateTime.Parse(ticket.ParkingResult?.EndTime ?? string.Empty);
                await _notifications.Send(
                    Lang.app_title,
                    $"{Lang.parking_started_ends_at} {dt.ToShortTimeString()}."
                );
                return;
            }
        }
        // fall back on sms
        if (useSmsParking && _sms.OngoingParking == null)
        {
            var nearby = await _sms.GetSiteInfoAsync();
            if (nearby is TollSiteInfo tollSite)
            {
                var ticket = await _sms.ParkAsync(plate, tollSite);
                if (ticket != null)
                {
                    await _notifications.Send(
                        Lang.app_title,
                        $"{Lang.sms_parking_started_in} {tollSite.Name}."
                    );
                }
            }
        }
    }
}
