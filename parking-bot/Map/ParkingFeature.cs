using Mapsui.Layers;

namespace ParkingBot.Map;

public class ParkingFeature : PointFeature
{
    public required Models.Parking.TollSiteInfo Site { get; set; }
    public ParkingFeature(double x, double y) : base(x, y) { }
}
