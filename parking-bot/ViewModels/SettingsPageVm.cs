using Microsoft.Extensions.Logging;

using ParkingBot.Models.Parking;
using ParkingBot.Properties;
using ParkingBot.Services;

using System.Collections.ObjectModel;

namespace ParkingBot.ViewModels;

public class SettingsPageVm(ILogger<SettingsPageVm> logger, KioskParkingService _kiosk) : BaseVm(logger)
{
    private string? _PhoneNumber;
    private bool _SendReminder;

    public ObservableCollection<ParkingSite> KioskList { get; } = [];
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
        return KioskList.First(site => site.SiteId.Equals(id)) != null;
    }

    internal async void AddKiosk(string id)
    {
        // TODO: get kiosk info from opendata by id and if exists store/add to list
        var info = await _kiosk.GetSiteInfoAsync(id);
        if (info != null)
        {
            var site = new ParkingSite(, info.)
            KioskSiteInfo
            KioskList.Add(info);
        }
        throw new NotImplementedException();
    }
}
