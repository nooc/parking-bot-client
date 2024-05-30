namespace ParkingBot.Services;

/* TODO KioskParkingService
{
public class KioskParkingService
{
    private static readonly string HISTORY_KEY = "kiosk-history";

    private readonly HttpClient _http;
    private readonly ILogger<KioskParkingService> _logger;
    private readonly IList<KioskParkingTicket> _history;

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

    public KioskParkingService(ILogger<KioskParkingService> logger, Http.HttpClientExt httpClient)
    {
        _http = httpClient;
        _logger = logger;
        _history = JsonSerializer.Deserialize<List<KioskParkingTicket>>(Preferences.Get(HISTORY_KEY, "[]")) ?? Enumerable.Empty<KioskParkingTicket>().ToList();
    }


    /// <summary>
    /// Get kiosk site info.
    /// </summary>
    /// <returns></returns>
    public async Task<KioskSiteInfo?> GetSiteInfoAsync(string externalId)
    {
        _logger.LogInformation("KioskParkingService.GetSiteInfoAsync()");
        return await _http.GetFromJsonAsync<KioskSiteInfo>($"{Values.GBG_BASE_KIOSK_URI}/external?externalId={externalId}");
    }

    public Task<KioskSiteStatus?> GetKioskSiteStatusAsync(string externalId)
    {
        _logger.LogInformation("KioskParkingService.GetKioskSiteStatusAsync()");
        return _http.GetFromJsonAsync<KioskSiteStatus>($"{Values.GBG_BASE_KIOSK_URI}/status?externalId={externalId}");
    }

    public async Task<ParkingTicket?> ParkAsync(KioskParkingRequest request, string externalId)
    {
        var result = await _http.PostAsJsonAsync($"{Values.GBG_BASE_KIOSK_URI}/external/assignment?externalId={externalId}", request);
        _logger.LogInformation("ParkAsync posted KioskParkingRequest and got {}.", result.StatusCode);

        if (result.IsSuccessStatusCode)
        {
            var dt = DateTime.UtcNow;
            var parking = new KioskParkingTicket
            {
                Start = dt,
                End = dt.AddHours(2), //TODO get from kiosk description
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
*/