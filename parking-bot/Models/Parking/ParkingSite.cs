using ParkingBot.Properties;

using Shiny;
using Shiny.Locations;

namespace ParkingBot.Models.Parking;

public record ParkingSite : GeofenceRegion
{
    public ISiteInfo? SiteInfo { get; set; }
    public bool Intersecting { get; set; } = false;
    public bool Parked { get; set; } = false;

    public ParkingSite(string Identifier, double lat, double lon)
        : base(Identifier, new Position(lat, lon), Distance.FromMeters(Values.GPS_REGION_RADIUS), false, true, true)
    { }
}
