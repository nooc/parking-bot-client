using Mapsui.Providers;

namespace ParkingBot.Map;

public class ProjectingFeatureProvider<T> : ProjectingProvider
    where T : IProvider
{
    private static readonly string SPHERICAL_MERCATOR = "EPSG:3857";

    public T Provider { get; }

    public ProjectingFeatureProvider(T provider) : base(provider)
    {
        CRS = SPHERICAL_MERCATOR;
        Provider = provider;
    }
}