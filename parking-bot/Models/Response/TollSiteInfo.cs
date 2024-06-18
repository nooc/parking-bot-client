using System.Text.Json.Serialization;

namespace ParkingBot.Models.Response;

public sealed class TollSiteInfo : Parking.ISiteInfo
{
    public string? Id { get; set; } // "429"
    public string? Name { get; set; } // "Kviberg Kasernområde"
    public string? Owner { get; set; } // "Göteborgs Stads Parkering"
    public int ParkingSpaces { get; set; } // 510
    public string? PhoneParkingCode { get; set; } // "4677"
    public string? ParkingCost { get; set; } // "8 kr/tim alla dagar 08-22, övrig tid 2 kr/tim, 50 kr/dag."
    public int CurrentParkingCost { get; set; } // 2
    public double Lat { get; set; } // 57.73619
    public double Long { get; set; } // 12.0314
    public string? WKT { get; set; } // ex "POINT (12.0314 57.73619)"

    [JsonIgnore]
    public long SiteId => long.Parse(Id ?? "-1");
    [JsonIgnore]
    public string Code => PhoneParkingCode ?? string.Empty;
    [JsonIgnore]
    public string SiteName => Name ?? string.Empty;
    [JsonIgnore]
    public string SiteDescription => $"<p>{Owner}</p><p>{ParkingCost}</p>";
    [JsonIgnore]
    public string SiteAvailability => $"{ParkingSpaces} {Properties.Lang.spaces}";
}
