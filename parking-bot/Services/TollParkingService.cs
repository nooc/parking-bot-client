using Microsoft.Extensions.Logging;

using ParkingBot.Models.Parking;
using ParkingBot.Properties;

using System.Text.Json;

namespace ParkingBot.Services;

public class TollParkingService
{
    private static readonly string HISTORY_KEY = "sms-history";
    private static readonly string ONGOING_SMS_KEY = "ongoing-sms";

    private readonly ILogger _logger;
    private readonly IList<TollParkingTicket> _history;
    private readonly SmsService _sms;
    private readonly GothenburgOpenDataService _opendata;

    public ParkingTicket? OngoingParking => GetOngoing();
    public IList<ParkingTicket> History => new List<ParkingTicket>();

    public TollParkingService(ILogger<TollParkingService> logger, GothenburgOpenDataService opendata, SmsService sms)
    {
        _logger = logger;
        _opendata = opendata;
        _history = JsonSerializer.Deserialize<List<TollParkingTicket>>(Preferences.Get(HISTORY_KEY, "[]")) ?? Enumerable.Empty<TollParkingTicket>().ToList();
        _sms = sms;
    }



    public async Task<ParkingTicket?> ParkAsync(string plate, TollSiteInfo site)
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
                Start = DateTime.UtcNow,
                PlateNumber = plate,
                ParkingResult = new SMSParkingResult()
            };
            _history.Insert(0, ticket);
            while (_history.Count > Values.MAX_HISTORY)
            {
                _history.RemoveAt(Values.MAX_HISTORY);
            }
            Preferences.Set(HISTORY_KEY, JsonSerializer.Serialize(_history));
            Preferences.Set(ONGOING_SMS_KEY, JsonSerializer.Serialize(ticket));

            return ticket;
        }
        return null;
    }

    private ParkingTicket? GetOngoing()
    {
        var saved = Preferences.Get(ONGOING_SMS_KEY, string.Empty);
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
    public async void StopParking()
    {
        var ongoing = OngoingParking as TollParkingTicket;
        if (ongoing != null)
        {
            var endParkingMessage = Values.GBG_SMS_STOP_TEMPLATE;
            if (endParkingMessage != null)
            {
                await _sms.SendMessage(
                    recipient: Values.GBG_SMS_NUMBER,
                    message: endParkingMessage,
                    tag: "stop");
            }
            ClearParking();
        }
    }

    internal void ClearParking()
    {
        Preferences.Remove(ONGOING_SMS_KEY);
    }
}
