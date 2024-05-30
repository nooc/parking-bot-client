using ParkingBot.Exceptions;
using ParkingBot.Models.Parking;
using ParkingBot.Properties;

using Shiny;
using Shiny.Jobs;
using Shiny.Locations;

namespace ParkingBot.Services;

public partial class ServiceHelperService(IServiceProvider services, ServiceData _data)
{
    private Lazy<IGeofenceManager> _geo = services.GetLazyService<IGeofenceManager>();
    private Lazy<IGpsManager> _gps = services.GetLazyService<IGpsManager>();
    private Lazy<IJobManager> _job = services.GetLazyService<IJobManager>();;
    private Lazy<VehicleBluetoothService> _bt = services.GetLazyService<VehicleBluetoothService>();
    private Lazy<TollParkingService> _toll = services.GetLazyService<TollParkingService>();
    private Lazy<AppService> _api = services.GetLazyService<AppService>();

    public void RequestAccess()
    {
        AssertAccessState("Bluetooth", _bt.Value.RequestAccessAsync());
        AssertAccessState("Background Jobs", _job.Value.RequestAccess());
        AssertAccessState("Location", _geo.Value.RequestAccess());
        AssertAccessState("Gps", _gps.Value.RequestAccess(GpsRequest.Realtime(true)));
    }

    private static async void AssertAccessState(string source, Task<AccessState> statusTask)
    {
        var status = await statusTask;
        switch (status)
        {
            case AccessState.Available:
                return;
            case AccessState.NotSupported:
            case AccessState.NotSetup:
                throw new ApplicationPermissionError(source, Lang.not_supported_msg);
            default:
                throw new ApplicationPermissionError(source, Lang.permission_error_msg);
        }
    }

    internal async void Start()
    {
        Preferences.Set(Values.SRV_IS_ACTIVE, true);
        await _api.Value.InitUser();
        _bt.Value.SetEnabled(true);
    }

    internal async Task StopAll()
    {
        Preferences.Set(Values.SRV_IS_ACTIVE, false);
        _job.Value.CancelAll();
        _bt.Value.SetEnabled(false);
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
