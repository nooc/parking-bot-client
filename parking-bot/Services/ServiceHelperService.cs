using ParkingBot.Exceptions;
using ParkingBot.Models;
using ParkingBot.Models.Parking;
using ParkingBot.Properties;

using Shiny;
using Shiny.Jobs;
using Shiny.Locations;

namespace ParkingBot.Services;

public partial class ServiceHelperService
{
    private readonly List<ParkingSite> Regions = [];
    private Api.PbData? _Settings = null;

    private Lazy<IGeofenceManager> _geo;
    private Lazy<IGpsManager> _gps;
    private Lazy<IJobManager> _job;
    private Lazy<VehicleBluetoothService> _bt;
    private Lazy<TollParkingService> _toll;
    private Lazy<AppService> _api;

    public ServiceHelperService(IServiceProvider services)
    {
        _geo = services.GetLazyService<IGeofenceManager>();
        _gps = services.GetLazyService<IGpsManager>();
        _job = services.GetLazyService<IJobManager>();
        _bt = services.GetLazyService<VehicleBluetoothService>();
        _toll = services.GetLazyService<TollParkingService>();
        _api = services.GetLazyService<AppService>();
    }

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
        if (_toll.Value.OngoingParking != null) _toll.Value.StopParking();
    }

    internal IList<ParkingSite> GetRegions()
    {
        return Regions;
    }

    internal async Task<Api.PbData?> GetSettings()
    {
        if (_Settings == null)
        {
            _Settings = await _api.Value.GetData();
        }
        return _Settings;
    }

    public bool IsServiceConfigured
    {
        get
        {
            // TODO: check if all services are configured
            return false;
        }
    }
}
