using Microsoft.Extensions.Logging;

using ParkingBot.Factories;
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
    private readonly ParkingSettings _settings;
    private readonly HttpClient _http;
    private readonly SmsService _sms;

    public ParkingTicket? OngoingParking => GetOngoing();
    public IList<ParkingTicket> History => new List<ParkingTicket>();

    public TollParkingService(ILogger<TollParkingService> logger, ParkingSettingsFactoryService parkingSettingsFactory, HttpClient http, SmsService sms)
    {
        _logger = logger;
        _history = JsonSerializer.Deserialize<List<SMSParkingTicket>>(Preferences.Get(HISTORY_KEY, "[]")) ?? Enumerable.Empty<SMSParkingTicket>().ToList();
        _settings = parkingSettingsFactory.Instance;
        _http = http;
        _sms = sms;
    }

    public async Task<ISiteInfo?> GetSiteInfoAsync()
    {
        TollSiteInfo? selectedSite = null;
        _logger.LogInformation("TollParkingService.GetSiteInfoAsync()");
        var loc = await Geolocation.Default.GetLastKnownLocationAsync();
        if (loc != null && _settings.Toll?.Endpoints is List<string> endpoints)
        {
            double selectedDist = 10000;
            foreach (var endpoint in endpoints)
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
            _settings.Toll?.StartTemplate ?? string.Empty,
            plate,
            site.PhoneParkingCode ?? string.Empty);
        var result = await _sms.SendMessage(
            recipient: _settings.Toll?.ServiceNumber ?? string.Empty,
            message: message ?? string.Empty,
            tag: "start");
        if (result)
        {
            var ticket = new SMSParkingTicket
            {
                Timestamp = DateTime.Now,
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
            .Replace("{APPID}", _settings.AppId)
            .Replace("{LATITUDE}", loc.Latitude.ToString())
            .Replace("{LONGITUDE}", loc.Longitude.ToString())
            .Replace("{RADIUS}", _settings.RegionRadius.ToString());
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
            var endParkingMessage = _settings.Toll?.EndTemplate;
            if (endParkingMessage != null && _settings.Toll?.ServiceNumber != null)
            {
                await _sms.SendMessage(
                    recipient: _settings.Toll.ServiceNumber,
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
