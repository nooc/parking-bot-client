namespace ParkingBot.Models.Parking;

public class TollSettings
{
    /// <summary>
    /// Each item is a template url. Replace {APPID}, {LATITUDE}, {LONGITUDE}, {RADIUS}.
    /// </summary>
    public List<string>? Endpoints { get; set; }
    public string? ServiceNumber { get; set; } // send sms to this number with parking code and plate number. Ex: 4972 ABC123
    public string? StartTemplate { get; set; } // send sms to this number with parking code and plate number. Ex: 4972 ABC123
    public string? EndTemplate { get; set; } // send sms to this number with parking code and plate number. Ex: 4972 ABC123
}