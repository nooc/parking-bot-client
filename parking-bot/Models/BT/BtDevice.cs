namespace ParkingBot.Models.Bt;

public class BtDevice(string deviceId, string deviceName)
{
    public string DeviceId { get; } = deviceId;
    public string DeviceName { get; set; } = deviceName;
}
