using ParkingBot.Models.Bt;

using Shiny.BluetoothLE;

namespace ParkingBot.Services;

public class VehicleBluetoothService(IBleManager _bt, GeoFencingService _geo, ServiceHelperService _hlp)
{
    private readonly Dictionary<string, CarBtDevice> ConnectedDict = [];

    public CarBtDevice? ConnectedCar
    {
        get
        {
            return ConnectedDict.FirstOrDefault().Value;
        }
    }

    public Task<Shiny.AccessState> RequestAccessAsync()
    {
        return _bt.RequestAccessAsync();
    }

    /// <summary>
    /// Get paired devices.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<BtDevice> GetPairedDevices()
    {
        var devices = _bt.TryGetPairedPeripherals();
        return devices.Select(peripheral => new BtDevice(peripheral.Uuid, peripheral.Name ?? "???"));
    }

    /// <summary>
    /// Get connected devices.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<BtDevice> GetConnectedDevices()
    {
        var devices = _bt.TryGetPairedPeripherals();
        return devices.Select(peripheral => new BtDevice(peripheral.Uuid, peripheral.Name ?? "???"));
    }

    /// <summary>
    /// Get registered devices.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<IEnumerable<CarBtDevice>> GetRegisteredDevices()
    {
        var settings = await _hlp.GetSettings();
        if (settings != null)
        {
            return settings.Vehicles.Select(v => new CarBtDevice(v.LicensePlate, v.DeviceId, v.Name));
        }
        return [];
    }

    /// <summary>
    /// Add device and trigger action.
    /// </summary>
    /// <param name="uuid"></param>
    /// <param name="name"></param>
    internal async void Connect(string uuid, string? name)
    {
        var regList = await GetRegisteredDevices() ?? [];
        foreach (var car in regList)
        {
            if (car.DeviceId == uuid)
            {
                ConnectedDict.Add(uuid, car);
            }
        }
    }

    /// <summary>
    /// Remove device and trigger action.
    /// </summary>
    /// <param name="uuid"></param>
    internal int Disconnect(string uuid)
    {
        ConnectedDict.Remove(uuid);
        return ConnectedDict.Count;
    }

    /// <summary>
    /// Enable or disable scan.
    /// If enabling scan while enabled, restart with refreshed device list.
    /// </summary>
    /// <param name="enabled"></param>
    internal async void SetEnabled(bool enabled)
    {
        if (enabled)
        {
            if (_bt.IsScanning) _bt.StopScan();
            // get devices to scan for and start scan
            var deviceEnum = await GetRegisteredDevices();
            var devices = deviceEnum.Select(dev => dev.DeviceId).ToArray() ?? [];
            if (devices.Length != 0)
            {
                _bt.ScanForUniquePeripherals(
                    new ScanConfig { ServiceUuids = devices });
            }
        }
        else
        {
            _bt.StopScan();
        }
    }
}
