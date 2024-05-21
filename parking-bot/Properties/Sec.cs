namespace ParkingBot.Properties;

internal class Sec
{
#if ANDROID
    public static readonly string GOOGLE_API_KEY = "AIzaSyCG9tMoWWMrHNVcm4KmPJbXO8XLzd_LnGM";
#elif IOS
    public static readonly string DUMMY = "";
#endif
}
