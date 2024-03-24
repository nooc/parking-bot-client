using System.ComponentModel;

namespace ParkingBot.Util;

public sealed class DateTimeUtils
{
    public static DayOfWeek FromStringDay(string day) => day switch
    {
        "Monday" => DayOfWeek.Monday,
        "Tuesday" => DayOfWeek.Tuesday,
        "Wednesday" => DayOfWeek.Wednesday,
        "Thursday" => DayOfWeek.Thursday,
        "Friday" => DayOfWeek.Friday,
        "Saturday" => DayOfWeek.Saturday,
        "Sunday" => DayOfWeek.Sunday,
        _ => throw new InvalidEnumArgumentException(day)
    };


}
