using ParkingBot.Exceptions;
using ParkingBot.Properties;

using Shiny;
using Shiny.BluetoothLE;
using Shiny.Jobs;
using Shiny.Locations;

using System.Text.RegularExpressions;

namespace ParkingBot.Services;

public class ServiceHelperService
{
    private readonly Regex phoneRe;
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

        phoneRe = new("^[\\+]?[(]?[0-9]{3}[)]?[-\\s\\.]?[0-9]{3}[-\\s\\.]?[0-9]{4,6}$");
    }

    public async Task<bool> HasSettingsPrerequisites()
    {
        string plate = Preferences.Get(Values.PARKING_LICENSEPLATE_KEY, string.Empty).Trim();
        string phone = Preferences.Get(Values.PARKING_PHONE_KEY, string.Empty).Trim();
        bool reminder = Preferences.Get(Values.PARKING_REMINDER_KEY, false);
        if (plate.IsEmpty())
        {
            if (Application.Current?.MainPage is Page page)
                await page.DisplayAlert(Lang.error, Lang.plate_format_error, Lang.exit);
            return false;
        }
        if (reminder && !phoneRe.IsMatch(phone))
        {
            if (Application.Current?.MainPage is Page page)
                await page.DisplayAlert(Lang.error, Lang.phone_format_error, Lang.exit);
            return false;
        }
        return true;

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

    public async Task StopAll()
    {
        _job.CancelAll();
        await _gps.StopListener();
        await _geo.StopAllMonitoring();
        if (_sms.OngoingParking != null) _sms.StopParking();
    }
}
