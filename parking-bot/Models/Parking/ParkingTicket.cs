using ParkingBot.Models.Response;

namespace ParkingBot.Models.Parking;

public abstract class ParkingTicket
{
    public required DateTime Started { get; set; }
    public DateTime? Ended { get; set; }
    public required string PlateNumber { get; set; }
    public TimeSpan Duration => (Ended == null ? DateTime.Now : (DateTime)Ended) - Started;
}

public sealed class KioskParkingTicket : ParkingTicket
{
    public KioskParkingResult? ParkingResult { get; set; }
}

public sealed class SMSParkingTicket : ParkingTicket
{
    public SMSParkingResult? ParkingResult { get; set; }
}
