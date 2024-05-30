using ParkingBot.Models.Parking;

using Shiny;
using Shiny.Locations;

namespace ParkingBot.Services;

public partial class GeoFencingService
{
    public record SiteRegion(ISiteInfo Site, string Identifier, Position Center, Distance Radius)
        : GeofenceRegion(Identifier, Center, Radius, false, true, true)
    {
        public GeofenceState State = GeofenceState.Unknown;
    }
}
