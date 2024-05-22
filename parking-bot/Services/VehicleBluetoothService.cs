using ParkingBot.Models.Bt;

using Shiny.BluetoothLE;

namespace ParkingBot.Services;

public class VehicleBluetoothService
{
    private readonly IBleManager _ble;

    public VehicleBluetoothService(IBleManager blueooth, ServiceHelperService access)
    {
        _ble = blueooth;
    }

    /// <summary>
    /// Get paired devices.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<BtDevice> GetPairedDevices()
    {
        var devices = _ble.TryGetPairedPeripherals();
        return devices.Select(peripheral => new BtDevice(peripheral.Uuid, peripheral.Name ?? "???"));
    }

    /// <summary>
    /// Get connected devices.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<BtDevice> GetConnectedDevices()
    {
        var devices = _ble.TryGetPairedPeripherals();
        return devices.Select(peripheral => new BtDevice(peripheral.Uuid, peripheral.Name ?? "???"));
    }

    /// <summary>
    /// Get registered devices.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<CarBtDevice> GetRegisteredDevices()
    {
        // TODO: Implement VehicleBluetoothService.GetRegisteredDevices
        throw new NotImplementedException();
    }

    /// <summary>
    /// Add device and trigger action.
    /// </summary>
    /// <param name="uuid"></param>
    /// <param name="name"></param>
    /// <exception cref="NotImplementedException"></exception>
    internal void Connect(string uuid, string? name)
    {
        // TODO: Implement VehicleBluetoothService.Connect
        throw new NotImplementedException();
    }

    /// <summary>
    /// Remove device and trigger action.
    /// </summary>
    /// <param name="uuid"></param>
    /// <exception cref="NotImplementedException"></exception>
    internal void Disconnect(string uuid)
    {
        // TODO: Implement VehicleBluetoothService.Disconnect
        throw new NotImplementedException();
    }
}
