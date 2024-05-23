using System.Text.RegularExpressions;

namespace ParkingBot.Util;

internal class VehicleUtils
{
    private static readonly Regex LicensePlateRegex = new(@"^[A-Z]{3}[0-9]{2}[A-Z0-9]$");

    public static Keyboard LicensePlateKeyboard => Keyboard.Create(KeyboardFlags.CapitalizeCharacter);

    public static bool IsValidLicensePlate(string licensePlate)
    {
        return LicensePlateRegex.IsMatch(licensePlate.Trim().ToUpper());
    }
}
