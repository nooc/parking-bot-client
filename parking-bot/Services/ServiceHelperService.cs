using ParkingBot.Exceptions;
using ParkingBot.Models.Parking;
using ParkingBot.Properties;

using Shiny;
using Shiny.BluetoothLE;
using Shiny.Jobs;
using Shiny.Locations;

namespace ParkingBot.Services;

public partial class ServiceHelperService
{
    private readonly List<ParkingSite> Regions = [];
    private readonly IGeofenceManager _geo;
    private readonly IGpsManager _gps;
    private readonly IJobManager _job;
    private readonly IBleManager _ble;
    private readonly TollParkingService _sms;

    public ServiceHelperService(IGeofenceManager geoFencer, IGpsManager gpsManager, IJobManager jobManager, IBleManager ble, TollParkingService sms)
    {
        _geo = geoFencer;
        _gps = gpsManager;
        _job = jobManager;
        _sms = sms;
        _ble = ble;
    }

    public void RequestAccess()
    {
        AssertAccessState("Bluetooth", _ble.RequestAccessAsync());
        AssertAccessState("Background Jobs", _job.RequestAccess());
        AssertAccessState("Location", _geo.RequestAccess());
        AssertAccessState("Gps", _gps.RequestAccess(GpsRequest.Realtime(true)));
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

    internal async Task StopAll()
    {
        _job.CancelAll();
        await _gps.StopListener();
        await _geo.StopAllMonitoring();
        if (_sms.OngoingParking != null) _sms.StopParking();
    }

    internal IList<ParkingSite> GetRegions()
    {
        return Regions;
    }

    internal void SyncSettings()
    {
        //TODO: sync settings, adding regions/parking spots
        //Regions.Add(region);
    }


}
