using ParkingBot.Properties;

using Shiny;
using Shiny.Locations;

namespace ParkingBot.Models.Parking;

public record ParkingSite : GeofenceRegion
{
    public enum SiteType
    {
        None,
        Time,
        Toll,
        Kios
    }

    public object? SiteData { get; set; }

    public GeofenceState State { get; set; } = GeofenceState.Unknown;

    public SiteType Type { get; set; } = SiteType.None;

    public ParkingSite(string Identifier, double lat, double lon)
        : base(Identifier, new Position(lat, lon), Distance.FromMeters(Values.GPS_REGION_RADIUS), false, true, true)
    { }
}
