using ParkingBot.Models;
using ParkingBot.Models.Bt;

namespace ParkingBot.Services;

public class VehicleBluetoothService(BluetoothHelper _bth, ServiceHelperService _hlp, ServiceData _data, AppService _api)
{
    private readonly Dictionary<string, CarBtDevice> ConnectedDict = [];

    public CarBtDevice? ConnectedCar
    {
        get
        {
            return ConnectedDict.FirstOrDefault().Value;
        }
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
    /// Add device.
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
    /// Remove device.
    /// </summary>
    /// <param name="uuid"></param>
    internal int Disconnect(string uuid)
    {
        ConnectedDict.Remove(uuid);
        return ConnectedDict.Count;
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
        foreach (var car in _data?.Settings?.Vehicles ?? [])
        {
            if (car.DeviceId == devId)
            {
                await _api.DeleteVehicle(car.Id);
                _hlp.GetSettings(force: true);
                return;
            }
        }
    }
}
