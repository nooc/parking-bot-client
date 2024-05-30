using ParkingBot.Models.Parking;
using ParkingBot.Properties;

using Shiny.Locations;
namespace ParkingBot.Util;
/*
 WKT examples:

LINESTRING (11.947624 57.719777, 11.947735 57.719776, 11.948311 57.719774, 11.948447 57.719772)

MULTILINESTRING ((11.951468 57.71786, 11.951526 57.717892), (11.951205 57.718133, 11.951112 57.718081), (11.951419 57.718036, 11.951275 57.717957), (11.951478 57.718008, 11.951332 57.717928), (11.951281 57.718108, 11.951134 57.718029))

GEOMETRYCOLLECTION (LINESTRING (11.952906 57.719792, 11.952791 57.719806, 11.952762 57.719809), POLYGON ((11.95281 57.720026, 11.95277 57.719876, 11.953205 57.719835, 11.953251 57.71999, 11.95281 57.720026)))

POINT (12.0314 57.73619)

 */
public class GeoTools
{
    public static bool Intersect(Position pos, double accuracy, ParkingSite site)
    {
        // TODO: intersect geometry
        var dist = pos.GetDistanceTo(site.Center).TotalMeters + accuracy;
        return dist <= Values.GPS_SITE_RADIUS;
    }
}
