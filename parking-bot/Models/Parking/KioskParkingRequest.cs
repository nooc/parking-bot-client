namespace ParkingBot.Models.Parking;

public sealed class KioskParkingRequest
{
    public required string externalId { get; set; }
    public required string registrationNumber { get; set; }
    public required string name { get; set; }
    public required string phoneNumber { get; set; }
    public required bool setEndTimeReminder { get; set; }
}
