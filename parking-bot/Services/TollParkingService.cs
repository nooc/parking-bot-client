using Microsoft.Extensions.Logging;

using ParkingBot.Models.Bt;
using ParkingBot.Models.Parking;
using ParkingBot.Properties;

using System.Text.Json;

namespace ParkingBot.Services;

public class TollParkingService(ILogger<TollParkingService> _logger, SmsService _sms, ServiceData _data, VehicleBluetoothService _bt)
{
    private static readonly string HISTORY_KEY = "toll.history";
    private static readonly string ONGOING_KEY = "ongoing.toll";

    public ParkingTicket? OngoingParking => GetOngoing();
    public IList<TollParkingTicket> History => JsonSerializer.Deserialize<List<TollParkingTicket>>(Preferences.Get(HISTORY_KEY, "[]")) ?? [];

    public async Task<TollParkingTicket?> ParkAsync(string plate, TollSiteInfo site)
    {
        var result = await _sms.SendMessage(
            recipient: Values.GBG_SMS_NUMBER,
            message: RenderMessage(
                Values.GBG_SMS_START_TEMPLATE,
                plate,
                site.PhoneParkingCode ?? string.Empty
                ),
            tag: "start");
        if (result)
        {
            var ticket = new TollParkingTicket
            {
                Uuid = Guid.NewGuid().ToString(),
                Start = DateTime.UtcNow,
                PlateNumber = plate,
                ParkingResult = new()
                {
                    AreaCode = site.PhoneParkingCode ?? string.Empty
                }
            };
            AddHistory(ticket);

            return ticket;
        }
        return null;
    }

    private TollParkingTicket? GetOngoing()
    {
        var saved = Preferences.Get(ONGOING_KEY, string.Empty);
        try
        {
            if (saved != null && !saved.Equals(string.Empty))
                return JsonSerializer.Deserialize<TollParkingTicket>(saved);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Invalid TollParkingTicket json");
        }
        return null;
    }


    private string RenderMessage(string template, string registrationPlate, string serviceNumber)
    {
        return template
            .Replace("{SITE_ID}", serviceNumber)
            .Replace("{PLATE_NUMBER}", registrationPlate);
    }

    /// <summary>
    /// Stop parking if ongoing parking, otherwise do nothing.
    /// </summary>
    public async void StopParking(ParkingSite site, bool force = false)
    {
        if (site.SiteInfo is TollSiteInfo info)
        {
            if (force)
            {
                // stop all
                var endParkingMessage = Values.GBG_SMS_STOP_TEMPLATE;
                await _sms.SendMessage(
                    recipient: Values.GBG_SMS_NUMBER,
                    message: endParkingMessage,
                    tag: "stop");
            }
            else if (_bt.ConnectedCar is CarBtDevice car)
            {
                // TODO: stop for connected car
                foreach (var ticket in History)
                {
                    if (ticket.PlateNumber == car.RegNumber)
                    {
                        var endParkingMessage = RenderMessage(Values.GBG_SMS_STOP_TEMPLATE_SPEC, ticket.PlateNumber, Values.GBG_SMS_NUMBER);
                        await _sms.SendMessage(
                            recipient: Values.GBG_SMS_NUMBER,
                            message: endParkingMessage,
                            tag: "stop");
                    }
                }
            }
        }
    }

    private void AddHistory(TollParkingTicket ticket)
    {
        var hist = History;
        hist.Insert(0, ticket);
        Preferences.Set(HISTORY_KEY, JsonSerializer.Serialize(hist));
    }

    private void UpdateHistory(TollParkingTicket ticket)
    {
        var hist = History;
        foreach (var item in hist)
        {
            if (ticket.ParkingResult)
        }
        Preferences.Set(HISTORY_KEY, JsonSerializer.Serialize(hist));
    }
}
