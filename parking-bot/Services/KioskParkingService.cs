using Microsoft.Extensions.Logging;

using ParkingBot.Factories;
using ParkingBot.Models.Parking;
using ParkingBot.Models.Response;
using ParkingBot.Properties;

using System.Net.Http.Json;
using System.Text.Json;

namespace ParkingBot.Services;

public class KioskParkingService
{
    private static readonly string HISTORY_KEY = "kiosk-history";

    private readonly HttpClient _http;
    private readonly ILogger<KioskParkingService> _logger;
    private readonly IList<KioskParkingTicket> _history;
    private readonly ParkingSettings _settings;

    public ParkingTicket? OngoingParking
    {
        get
        {
            try
            {
                if (_history.Count() != 0)
                {
                    var parking = _history.First();
                    var endTime = DateTime.Parse(parking.ParkingResult?.EndTime ?? string.Empty);
                    if (endTime.CompareTo(DateTime.Now) > 0) return parking;
                }
            }
            catch { }
            return null;
        }
    }

    public IList<ParkingTicket> History => _history.Select(item => item as ParkingTicket).ToList();

    public KioskParkingService(ILogger<KioskParkingService> logger, HttpClient httpClient, ParkingSettingsFactoryService parkingSettingsFactory)
    {
        _http = httpClient;
        _logger = logger;
        _history = JsonSerializer.Deserialize<List<KioskParkingTicket>>(Preferences.Get(HISTORY_KEY, "[]")) ?? Enumerable.Empty<KioskParkingTicket>().ToList();
        _settings = parkingSettingsFactory.Instance;
    }


    /// <summary>
    /// Get current site info.
    /// If weekend, get kiosk variant, else toll variant.
    /// </summary>
    /// <returns></returns>
    public async Task<ISiteInfo?> GetSiteInfoAsync(KioskSite site)
    {
        _logger.LogInformation("KioskParkingService.GetSiteInfoAsync()");
        return await _http.GetFromJsonAsync<KioskSiteInfo>($"{_settings.Kiosk?.Endpoint}/external?externalId={site.ExternalId}");
    }

    public Task<KioskSiteStatus?> GetKioskSiteStatusAsync(KioskSite site)
    {
        _logger.LogInformation("KioskParkingService.GetKioskSiteStatusAsync()");
        return _http.GetFromJsonAsync<KioskSiteStatus>($"{_settings.Kiosk?.Endpoint}/status?externalId={site.ExternalId}");
    }

    public async Task<ParkingTicket?> ParkAsync(KioskParkingRequest request, KioskSite site)
    {
        var result = await _http.PostAsJsonAsync($"{_settings.Kiosk?.Endpoint}/external/assignment?externalId={site.ExternalId}", request);
        _logger.LogInformation("ParkAsync posted KioskParkingRequest and got {}.", result.StatusCode);

        if (result.IsSuccessStatusCode)
        {
            var parking = new KioskParkingTicket
            {
                Timestamp = DateTime.Now,
                ParkingResult = await result.Content.ReadFromJsonAsync<KioskParkingResult>(),
                PlateNumber = request.registrationNumber
            };
            _history.Insert(0, parking);
            while (_history.Count > Values.MAX_HISTORY)
            {
                _history.RemoveAt(Values.MAX_HISTORY);
            }
            Preferences.Set(HISTORY_KEY, JsonSerializer.Serialize(_history));
            return parking;
        }
        else
        {
            return null;
        }
    }
}
