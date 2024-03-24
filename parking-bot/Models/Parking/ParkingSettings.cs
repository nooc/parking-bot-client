namespace ParkingBot.Models.Parking;

public class ParkingSettings
{
    // App id for data.goteborg.se
    public string? AppId { get; set; }
    public KioskSettings? Kiosk { get; set; }
    public TollSettings? Toll { get; set; }
    public FreeSettings? Free { get; set; }
    public int MinGpsAccuracy { get; set; }
    public int MaxGpsDistance { get; set; }
    public int RegionRadius { get; set; }

}
