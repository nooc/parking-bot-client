namespace ParkingBot.Models.Bt;

public class CarBtDevice : BtDevice
{
    public string RegNumber { get; set; }

    public CarBtDevice(string regNum, BtDevice dev) : base(dev.DeviceId, dev.DeviceName)
    {
        RegNumber = regNum;
    }
    public CarBtDevice(string regNum, string devId, string devName) : base(devId, devName)
    {
        RegNumber = regNum;
    }
}
