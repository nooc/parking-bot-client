namespace ParkingBot.Models;

public sealed class Api
{
    public class PbUpdateUser
    {
        public string Phone { get; set; } = string.Empty;
    }

    public class PbUser
    {
        public string Id { get; set; } = string.Empty;
        public int State { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string[] Roles { get; set; } = [];
    }

    public class PbVehicle
    {
        public long Id { get; set; }
        public required string DeviceId { get; set; }
        public required string LicensePlate { get; set; }
        public required string Name { get; set; }
    }

    public class PbAddVehicle
    {
        public required string DeviceId { get; set; }
        public required string LicensePlate { get; set; }
        public required string Name { get; set; }
    }
    public class PbUpdateVehicle
    {
        public required string Name { get; set; }
    }

    public class PbTollParking
    {
        public long Id { get; set; }
        public required string CarParkId { get; set; }
        public required string PhoneParkingCode { get; set; }
    }

    public class PbKioskParking
    {
        public long Id { get; set; }
        public required string KioskId { get; set; }
    }

    public class PbCarParks
    {
        public required List<PbTollParking> Toll { get; set; }
        public required List<PbKioskParking> Kiosk { get; set; }
    }

    public class PbData
    {
        public required PbUser User { get; set; }
        public required List<PbVehicle> Vehicles { get; set; }
        public required PbCarParks CarParks { get; set; }
    }

    public class ParkingOperationLog
    {
        public required long Id { get; set; }
        public required string UserId { get; set; }
        public required string ParkingCode { get; set; }
        public required string DeviceId { get; set; }
        public required string LicensePlate { get; set; }
        public required string Phone { get; set; }
        public required string Type { get; set; }
        public required long Start { get; set; }
        public required long Stop { get; set; }
    }
    public class ParkingLogCreate
    {
        public required string ParkingCode { get; set; }
        public required string DeviceId { get; set; }
        public required string LicensePlate { get; set; }
        public required string Type { get; set; }
        public required long Start { get; set; }
        public required long Stop { get; set; }
    }
}
