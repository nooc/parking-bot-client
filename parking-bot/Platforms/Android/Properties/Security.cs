using static Android.Provider.Settings;

namespace ParkingBot.Properties;

internal class Security
{
    // On Android 8.0 (API level 26) and higher versions of the platform,
    // a 64-bit number(expressed as a hexadecimal string), unique to each
    // combination of app-signing key, user, and device.
    public static string API_ID
    {
        get
        {
            return Secure.GetString(Platform.AppContext.ContentResolver ?? null, Secure.AndroidId) ?? "dummy";
        }
    }
}
