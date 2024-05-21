using ParkingBot.Properties;

namespace ParkingBot.ViewModels;

public class SettingsPageVm : BaseVm
{
    private string? _LicensePlate;
    private string? _PhoneNumber;
    private bool _SendReminder;
    private bool _SmsParking;

    public string? LicensePlate
    {
        get => _LicensePlate;
        set { StoreStringProperty(value, Values.PARKING_LICENSEPLATE_KEY); SetProperty(ref _LicensePlate, value); }
    }
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
    public bool SmsParking
    {
        get => _SmsParking;
        set { StorBoolProperty(value, Values.PARKING_USE_SMS_PARKING_KEY); SetProperty(ref _SmsParking, value); OnPropertyChanged(nameof(SmsStateLabel)); }
    }
    public string ReminderStateLabel => _SendReminder ? Lang.on : Lang.off;
    public string SmsStateLabel => _SmsParking ? Lang.on : Lang.off;

    public SettingsPageVm() : base() { }

    private void StoreStringProperty(string? _val, string propertyName)
    {
        Preferences.Set(propertyName, _val?.Trim());
    }
    private void StorBoolProperty(bool _val, string propertyName)
    {
        Preferences.Set(propertyName, _val);
    }

    protected override void ExecuteLoadModelCommand()
    {
        _LicensePlate = Preferences.Get(Values.PARKING_LICENSEPLATE_KEY, null);
        _PhoneNumber = Preferences.Get(Values.PARKING_PHONE_KEY, null);
        _SendReminder = Preferences.Get(Values.PARKING_REMINDER_KEY, false);
        _SmsParking = Preferences.Get(Values.PARKING_USE_SMS_PARKING_KEY, false);
    }
}
