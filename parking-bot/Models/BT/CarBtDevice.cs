namespace ParkingBot.Models.Bt;

public class CarBtDevice : BtDevice
{
    public string RegNumber { get; set; }

    public CarBtDevice(string regNumber, BtDevice dev) : base(dev.DeviceId, dev.DeviceName)
    {
        RegNumber = regNumber;
    }
}
