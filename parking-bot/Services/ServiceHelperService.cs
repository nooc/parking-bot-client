using ParkingBot.Properties;

using Shiny;
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
    private readonly TollParkingService _sms;

    public ServiceHelperService(IGeofenceManager geoFencer, IGpsManager gpsManager, IJobManager jobManager, TollParkingService sms)
    {
        _geo = geoFencer;
        _gps = gpsManager;
        _job = jobManager;
        _sms = sms;

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

    public async Task<bool> RequestAccess()
    {
        var jobaccess = await _job.RequestAccess();
        var geoaccess = await _geo.RequestAccess();
        var gpsaccess = await _gps.RequestAccess(GpsRequest.Realtime(true));
        if (geoaccess != AccessState.Available || gpsaccess != AccessState.Available
            || jobaccess != AccessState.Available)
        {
            if (Application.Current?.MainPage is Page page)
                await page.DisplayAlert(Lang.insuf_perm, Lang.perm_not_met, Lang.exit);
            return false;
        }
        return true;
    }

    public async Task StopAll()
    {
        _job.CancelAll();
        await _gps.StopListener();
        await _geo.StopAllMonitoring();
        if (_sms.OngoingParking != null) _sms.StopParking();
    }
}
