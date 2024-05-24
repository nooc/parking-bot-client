using Microsoft.Extensions.Logging;

using ParkingBot.Models.Parking;
using ParkingBot.Properties;

using System.Net.Http.Json;
using System.Text.Json;

namespace ParkingBot.Services;

public class TollParkingService
{
    private static readonly string HISTORY_KEY = "sms-history";
    private static readonly string ONGOING_SMS_KEY = "ongoing-sms";

    private readonly ILogger _logger;
    private readonly IList<SMSParkingTicket> _history;
    private readonly HttpClient _http;
    private readonly SmsService _sms;
    private readonly string[] Endpoints = [Values.GBG_SERVICE_PRIV_TOLL_URI, Values.GBG_SERVICE_PUBL_TOLL_URI];

    public ParkingTicket? OngoingParking => GetOngoing();
    public IList<ParkingTicket> History => new List<ParkingTicket>();

    public TollParkingService(ILogger<TollParkingService> logger, HttpClient http, SmsService sms)
    {
        _logger = logger;
        _history = JsonSerializer.Deserialize<List<SMSParkingTicket>>(Preferences.Get(HISTORY_KEY, "[]")) ?? Enumerable.Empty<SMSParkingTicket>().ToList();
        _http = http;
        _sms = sms;
    }

    public async Task<ISiteInfo?> GetSiteInfoAsync()
    {
        TollSiteInfo? selectedSite = null;
        _logger.LogInformation("TollParkingService.GetSiteInfoAsync()");
        var loc = await Geolocation.Default.GetLastKnownLocationAsync();
        if (loc != null)
        {
            double selectedDist = 10000;
            foreach (var endpoint in Endpoints)
            {
                var sites = await _http.GetFromJsonAsync<List<TollSiteInfo>>(RenderUrl(endpoint, loc));
                if (sites != null)
                {
                    foreach (var site in sites)
                    {
                        if (site != null)
                        {
                            var dist = Location.CalculateDistance(loc.Latitude, loc.Longitude, site.Lat, site.Long, DistanceUnits.Kilometers) * 1000;
                            if (dist < selectedDist)
                            {
                                selectedDist = dist;
                                selectedSite = site;
                            }
                        }
                    }
                }
            }
        }
        return selectedSite;
    }

    public async Task<ParkingTicket?> ParkAsync(string plate, TollSiteInfo site)
    {
        var message = RenderMessage(
            Values.GBG_SMS_START_TEMPLATE ?? string.Empty,
            plate,
            site.PhoneParkingCode ?? string.Empty);
        var result = await _sms.SendMessage(
            recipient: Values.GBG_SMS_NUMBER ?? string.Empty,
            message: message ?? string.Empty,
            tag: "start");
        if (result)
        {
            var ticket = new SMSParkingTicket
            {
                Started = DateTime.Now,
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
                return JsonSerializer.Deserialize<SMSParkingTicket>(saved);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Invaild SMSParkingTicket json");
        }
        return null;
    }

    private string RenderUrl(string template, Location loc)
    {

        return template
            .Replace("{APPID}", Values.GBG_APP_ID)
            .Replace("{LATITUDE}", loc.Latitude.ToString())
            .Replace("{LONGITUDE}", loc.Longitude.ToString())
            .Replace("{RADIUS}", Values.GPS_REGION_RADIUS.ToString());
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
        var ongoing = OngoingParking as SMSParkingTicket;
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
