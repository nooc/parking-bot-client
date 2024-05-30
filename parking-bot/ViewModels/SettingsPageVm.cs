using Microsoft.Extensions.Logging;

using ParkingBot.Models.Parking;
using ParkingBot.Properties;

using System.Collections.ObjectModel;

namespace ParkingBot.ViewModels;

public class SettingsPageVm(ILogger<SettingsPageVm> logger
    //KioskParkingService kiosk,
    //GothenburgOpenDataService opendata
    ) : BaseVm(logger)
{
    private string? _PhoneNumber;
    private bool _SendReminder;

    internal ObservableCollection<ParkingSite> KioskList { get; } = [];

    public string? PhoneNumber
    {
        get => _PhoneNumber;
        set { StoreStringProperty(value, Values.PARKING_PHONE_KEY); SetProperty(ref _PhoneNumber, value); }
    }
    public bool SendReminder
    {
        get => _SendReminder;
        set { StorBoolProperty(value, Values.PARKING_REMINDER_KEY); SetProperty(ref _SendReminder, value); OnPropertyChanged(nameof(ReminderStateLabel)); }
    }
    public string ReminderStateLabel => _SendReminder ? Lang.on : Lang.off;

    private static void StoreStringProperty(string? _val, string propertyName)
    {
        Preferences.Set(propertyName, _val?.Trim());
    }
    private static void StorBoolProperty(bool _val, string propertyName)
    {
        Preferences.Set(propertyName, _val);
    }

    protected override void ExecuteLoadModelCommand()
    {
        _PhoneNumber = Preferences.Get(Values.PARKING_PHONE_KEY, null);
        _SendReminder = Preferences.Get(Values.PARKING_REMINDER_KEY, false);
    }

    internal bool HasKiosk(string id)
    {
        //TODO: identifier  is..?
        return KioskList.First(site => site.Identifier.Equals(id)) != null;
    }

    internal void AddKiosk(string id)
    {
        // TODO: get kiosk info from opendata by id and if exists store/add to list
        /*
        var info = await kiosk.GetSiteInfoAsync(id);
        if (info != null)
        {
            var result = await opendata.GetSiteInfoAsync(info.);
            if (result != null)
            {
                var site = new ParkingSite(info.SiteId, result.)
                site.SiteData = info;
                site.Type = ParkingType.Kiosk;
                KioskList.Add(site);
            }
        }*/
        throw new NotImplementedException();
    }
}
