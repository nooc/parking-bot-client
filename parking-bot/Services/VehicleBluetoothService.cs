using ParkingBot.Models;
using ParkingBot.Models.Bt;

using Shiny.BluetoothLE;

namespace ParkingBot.Services;

public class VehicleBluetoothService(IBleManager _bt, BluetoothHelper _bth, ServiceHelperService _hlp, ServiceData _data, AppService _api)
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
        return _bth.GetPairedDevices();
    }

    /// <summary>
    /// Get connected devices.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<BtDevice> GetConnectedDevices()
    {
        return _bth.GetConnectedDevices();
    }

    /// <summary>
    /// Get registered devices.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<CarBtDevice> GetRegisteredDevices(bool force = false)
    {
        _hlp.GetSettings(force);
        return _data.Settings?.Vehicles.Select(v => new CarBtDevice(v.LicensePlate, v.DeviceId, v.Name)) ?? [];
    }

    /// <summary>
    /// Add device and trigger action.
    /// </summary>
    /// <param name="uuid"></param>
    /// <param name="name"></param>
    internal void Connect(string uuid, string? name)
    {
        var regList = GetRegisteredDevices() ?? [];
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
    internal void SetEnabled(bool enabled)
    {
        if (enabled)
        {
            if (_bt.IsScanning) _bt.StopScan();
            // get devices to scan for and start scan
            var deviceEnum = GetRegisteredDevices();
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

    /// <summary>
    /// Add registered device.
    /// </summary>
    /// <param name="device"></param>
    public async void RegisterCar(CarBtDevice device)
    {
        var add = new Api.PbAddVehicle
        {
            DeviceId = device.DeviceId,
            LicensePlate = device.RegNumber,
            Name = device.DeviceName
        };
        var result = await _api.AddVehicle(add);
        if (result is Api.PbVehicle vehicle)
        {
            _hlp.GetSettings(force: true);
        }
    }

    /// <summary>
    /// Remove by bt device id
    /// </summary>
    /// <param name="devId"></param>
    public async void RemoveCar(string devId)
    {
        var found = _data?.Settings?.Vehicles.Where(item => item.DeviceId == devId).FirstOrDefault();
        if (found != null)
        {
            await _api.DeleteVehicle(found.Id);
            _hlp.GetSettings(force: true);
        }
    }
}
