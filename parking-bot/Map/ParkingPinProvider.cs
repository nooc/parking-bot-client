using Mapsui.Providers;

namespace ParkingBot.Map;

public class ProjectingPinProvider : ProjectingProvider
{
    private static readonly string SPHERICAL_MERCATOR = "EPSG:3857";

    public PinProvider PinProvider { get; }

    public ProjectingPinProvider(PinProvider provider) : base(provider)
    {
        CRS = SPHERICAL_MERCATOR;
        PinProvider = provider;
    }
}