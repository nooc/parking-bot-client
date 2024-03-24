namespace ParkingBot.Properties;

public sealed class Values
{
    // Jobs
    public static readonly string PARKING_JOB = "ParkingBot.Park";
    // Preferences
    public static readonly string PARKING_LICENSEPLATE_KEY = "parking.licenseplace";
    public static readonly string PARKING_PHONE_KEY = "parking.smsnumber";
    public static readonly string PARKING_REMINDER_KEY = "parking.reminder";
    public static readonly string PARKING_USE_SMS_PARKING_KEY = "parking.use-sms";
    public static readonly string PARKING_SMS_PARKING_KEY = "parking.sms";
    public static readonly string TOKEN_VERIFIED_KEY = "token.verivied";
    public static readonly string TOKEN_USER_KEY = "token.user";
    // Misc
    public static readonly int MAX_HISTORY = 10;

    public static string USER_AGENT => $"ParkingBot/1.0 ({DeviceInfo.Current.Platform})";
}
