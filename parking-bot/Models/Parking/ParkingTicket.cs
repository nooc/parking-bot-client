namespace ParkingBot.Models.Parking;

public abstract class ParkingTicket
{
    public required DateTime Start { get; set; }
    public DateTime? Stop { get; set; }
    public required string PlateNumber { get; set; }
    public TimeSpan Duration => (Stop == null ? DateTime.Now : (DateTime)Stop) - Start;
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
