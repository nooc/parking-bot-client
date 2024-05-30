namespace ParkingBot.Models.Response;
/* TODO KioskSiteInfo
public sealed class KioskSiteInfo : ISiteInfo
{
    public string? ExternalId { get; set; }
    public string? Name { get; set; }
    public string? Heading { get; set; }
    public string? Description { get; set; }
    public string? ParkingFullDescription { get; set; }
    public string? ParkingFullHeading { get; set; }
    public string? ParkingName { get; set; }
    public int ParkingAreaId { get; set; }
    public int AvailablePermitsCount { get; set; }
    public string? MaxTime { get; set; } // ex: "2 hours"

    [JsonIgnore]
    public long SiteId => ParkingAreaId;
    [JsonIgnore]
    public string SiteName => Name ?? "???";
    [JsonIgnore]
    public string SiteDescription => Description ?? "???";
    [JsonIgnore]
    public string SiteAvailability => $"{AvailablePermitsCount} {Properties.Lang.available}";
}
*/