using ParkingBot.Models.Parking;
using ParkingBot.Properties;

using Shiny;
using Shiny.Jobs;
using Shiny.Locations;

namespace ParkingBot.Services;

public partial class ServiceHelperService(IServiceProvider services, ServiceData _data)
{
    private readonly Lazy<IGeofenceManager> _geo = services.GetLazyService<IGeofenceManager>();
    private readonly Lazy<IGpsManager> _gps = services.GetLazyService<IGpsManager>();
    private readonly Lazy<IJobManager> _job = services.GetLazyService<IJobManager>();
    private readonly Lazy<TollParkingService> _toll = services.GetLazyService<TollParkingService>();
    private readonly Lazy<AppService> _api = services.GetLazyService<AppService>();
    private readonly Lazy<BluetoothHelper> _bth = services.GetLazyService<BluetoothHelper>();
    private readonly Lazy<GeoFencingService> _geos = services.GetLazyService<GeoFencingService>();

    internal async void Start()
    {
        bool init = await _api.Value.InitUser();
        bool geo = false;
        if (init) geo = await _geos.Value.SetEnabled(true);
        Preferences.Set(Values.SRV_IS_ACTIVE, geo);
    }

    /// <summary>
    /// Stopp all monitoring services and any active parking.
    /// </summary>
    /// <returns></returns>
    internal async Task StopAll()
    {
        Preferences.Set(Values.SRV_IS_ACTIVE, false);
        _job.Value.CancelAll();
        await _gps.Value.StopListener();
        await _geo.Value.StopAllMonitoring();
        foreach (var (k, v) in _data.ParkingSites)
        {
            if (v is ParkingSite site && site.Parked)
            {
                _toll.Value.StopParking(site);
            }
        }
    }

    /// <summary>
    /// Get settings from api.
    /// </summary>
    /// <param name="force"></param>
    internal async void GetSettings(bool force = false)
    {
        if (!(_data.Settings == null || force)) return;

        _data.Settings = await _api.Value.GetData();
    }

    public bool IsServiceConfigured
    {
        get
        {
            return _data.Settings != null;
        }
    }
}
