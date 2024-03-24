namespace ParkingBot.Models.Parking;

public interface ISiteInfo
{
    long SiteId { get; }
    string SiteName { get; }
    string SiteDescription { get; }
    string SiteAvailability { get; }
}
