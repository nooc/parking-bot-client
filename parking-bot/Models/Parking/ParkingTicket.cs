using ParkingBot.Models.Response;
using ParkingBot.Properties;

namespace ParkingBot.Models.Parking;

public abstract class ParkingTicket
{
    public DateTime Timestamp { get; set; }
    public string? PlateNumber { get; set; }

    public abstract string GetSummary();
}

public sealed class KioskParkingTicket : ParkingTicket
{
    public KioskParkingResult? ParkingResult { get; set; }

    public override string GetSummary()
    {
        var end = DateTime.Parse(ParkingResult?.EndTime ?? string.Empty);
        return $"{Timestamp}\n{PlateNumber}: {Lang.ends_at} -> {end.ToShortTimeString()}";
    }
}

public sealed class SMSParkingTicket : ParkingTicket
{
    public SMSParkingResult? ParkingResult { get; set; }

    public override string GetSummary()
    {
        return "???";
    }
}
