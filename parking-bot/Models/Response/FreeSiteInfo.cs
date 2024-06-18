namespace ParkingBot.Models.Response;

public sealed class FreeSiteInfo
{
    public string? Id { get; set; }
    public string? Name { get; set; }

    public string? Owner { get; set; }

    public int ParkingSpaces { get; set; } // ex 2147483647

    public string? MaxParkingTime { get; set; } // String content",

    public string? MaxParkingTimeLimitation { get; set; } // String content",

    public string? ExtraInfo { get; set; } // String content",

    public int Distance { get; set; } // 2147483647,

    public double Lat { get; set; } // ex 1.26743233E+15

    public string? Long { get; set; } // ex 1.26743233E+15

    public string? WKT { get; set; } // String content"
}
