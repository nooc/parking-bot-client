namespace ParkingBot.Models.Parking;

public interface ISiteInfo
{
    long SiteId { get; }
    string Code { get; }
    string SiteName { get; }
    string SiteDescription { get; }
    string SiteAvailability { get; }
}
