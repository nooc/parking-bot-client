namespace ParkingBot.Models.Parking;

public class KioskSite
{
    public long SiteId { get; set; }
    public string? Title { get; set; }
    public string? ExternalId { get; set; }
    public List<string>? Days { get; set; }
    public SiteLocation? Location { get; set; }
    public double Radius { get; set; }
    public Dictionary<string, string>? Description { get; set; }
}