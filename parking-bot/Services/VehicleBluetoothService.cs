using ParkingBot.Models.Bt;

using Shiny.BluetoothLE;

namespace ParkingBot.Services;

public class VehicleBluetoothService
{
    private readonly IBleManager _blueooth;

    public VehicleBluetoothService(IBleManager blueooth, ServiceHelperService access)
    {
        _blueooth = blueooth;
    }

    public IEnumerable<BtDevice> GetPairedDevices()
    {
        var devices = _blueooth.TryGetPairedPeripherals();
        return devices.Select(peripheral => new BtDevice { DeviceId = peripheral.Uuid, DeviceName = peripheral.Name ?? "???" });
    }
    public IEnumerable<BtDevice> GetConnectedDevices()
    {
        var devices = _blueooth.TryGetPairedPeripherals();
        return devices.Select(peripheral => new BtDevice { DeviceId = peripheral.Uuid, DeviceName = peripheral.Name ?? "???" });
    }
}
