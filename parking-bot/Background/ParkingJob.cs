using Microsoft.Extensions.Logging;

using ParkingBot.Models.Bt;
using ParkingBot.Models.Parking;
using ParkingBot.Properties;
using ParkingBot.Services;

using Shiny.Jobs;
using Shiny.Notifications;

using System.Text.Json;

namespace ParkingBot.Background;

public class ParkingJob : Job
{
    private class RunContext
    {
        public required CarBtDevice Car { get; set; }
        public required string Site { get; set; }
        public required string SiteType { get; set; }
        public required string Phone { get; set; }
        public ParkingTicket? Ticket { get; set; }
    }

    private readonly ILogger<ParkingJob> _logger;
    private readonly INotificationManager _notifications;
    //private readonly KioskParkingService _kiosk;
    private readonly TollParkingService _sms;
    private readonly ServiceHelperService _helper;

    public ParkingJob(ILogger<ParkingJob> logger, INotificationManager notificationManager,
        //KioskParkingService kioskParkingService,
        TollParkingService smsParkingService, ServiceHelperService helper) : base(logger)
    {
        _logger = logger;
        _notifications = notificationManager;
        //_kiosk = kioskParkingService;
        _sms = smsParkingService;
        _helper = helper;
    }

    protected override async Task Run(CancellationToken cancelToken)
    {
        if (_helper.IsServiceConfigured)
        {
            if (JobInfo.Parameters is Dictionary<string, string> dict)
            {
                var ctx = new RunContext
                {
                    Car = JsonSerializer.Deserialize<CarBtDevice>(dict["car"]) ?? throw new ArgumentNullException("JobInfo.Parameters:car"),
                    Phone = dict["phone"],
                    Site = dict["site"],
                    SiteType = dict["site_type"]
                };

                if (ctx.SiteType.Equals("toll"))
                {
                    TryTollPark(ctx);

                }
                else if (ctx.SiteType.Equals("kiosk"))
                {
                    TryKioskPark(ctx);
                }
            }
        }
        else
        {
            await _helper.StopAll();
            await _notifications.Send(new Notification
            {
                Title = Lang.auto_parking,
                Message = Lang.settings_error
            });
            return;
        }
    }

    private async void TryTollPark(RunContext ctx)
    {
        var tollSite = JsonSerializer.Deserialize<TollSiteInfo>(ctx.Site) ?? throw new ArgumentNullException("RunContext.Site");
        var ticket = await _sms.ParkAsync(ctx.Car.RegNumber, tollSite);
        if (ticket != null)
        {
            await _notifications.Send(
                Lang.app_title,
                Lang.parking_started
            );
        }
    }

    private void TryKioskPark(RunContext ctx)
    {
        /*
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
        */
    }
}
