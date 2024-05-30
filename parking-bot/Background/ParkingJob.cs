using Microsoft.Extensions.Logging;

using ParkingBot.Models.Parking;
using ParkingBot.Properties;
using ParkingBot.Services;

using Shiny.Jobs;
using Shiny.Notifications;

namespace ParkingBot.Background;

public class ParkingJob(ILogger<ParkingJob> _logger, INotificationManager _notif, TollParkingService _toll, ServiceData _data) : Job(_logger)
{
    private class RunContext
    {
        public ParkingSite? Site { get; set; }
        public required string RegNum { get; set; }
        public ParkingTicket? Ticket { get; set; }
    }

    protected override Task Run(CancellationToken cancelToken)
    {
        var identifier = JobInfo.Parameters?["site"] ?? string.Empty;
        var site = _data.ParkingSites[identifier];
        var action = JobInfo.Parameters?["action"] ?? string.Empty;
        var ctx = new RunContext
        {
            Site = site,
            RegNum = JobInfo.Parameters?["car"] ?? string.Empty,
        };

        if (action == "start")
        {
            if (site.SiteInfo is TollSiteInfo) TryTollPark(ctx);
            //else (info is KioskSiteInfo) TryKioskPark(ctx);
        }
        else if (action == "stop")
        {
            if (site.SiteInfo is TollSiteInfo) TryStopTollPark(ctx);
        }
        return Task.CompletedTask;
    }
    private async void TryTollPark(RunContext ctx)
    {
        if (ctx.Site?.SiteInfo is TollSiteInfo toll)
        {
            ctx.Ticket = await _toll.ParkAsync(ctx.RegNum, toll);
            if (ctx.Ticket != null)
            {
                await _notif.Send(
                    Lang.app_title,
                    Lang.parking_started
                );
            }
        }
    }
    private async void TryStopTollPark(RunContext ctx)
    {
        if (ctx.Site?.SiteInfo is TollSiteInfo toll)
        {
            _toll.StopParking(ctx.Site);
            await _notif.Send(
                Lang.app_title,
                Lang.parking_stopped
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
