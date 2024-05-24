using System.Text.RegularExpressions;

namespace ParkingBot.Util;

internal partial class RegexUtils
{
    [GeneratedRegex("^[A-Z]{3}[0-9]{2}[A-Z0-9]$")]
    public static partial Regex LicensePlateRegex();

    [GeneratedRegex("^[\\+]?[(]?[0-9]{3}[)]?[-\\s\\.]?[0-9]{3}[-\\s\\.]?[0-9]{4,6}$")]
    public static partial Regex PhoneRegex();
}
