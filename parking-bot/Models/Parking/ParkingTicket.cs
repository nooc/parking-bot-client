namespace ParkingBot.Models.Parking;

public abstract class ParkingTicket
{
    public required DateTime Start { get; set; }
    public DateTime? End { get; set; }
    public required string PlateNumber { get; set; }
    public TimeSpan Duration => (End == null ? DateTime.Now : (DateTime)End) - Start;
}
/*
public sealed class KioskParkingTicket : ParkingTicket
{
    public KioskParkingResult? ParkingResult { get; set; }
}
*/

public sealed class TollParkingTicket : ParkingTicket
{
    public SMSParkingResult? ParkingResult { get; set; }
}
