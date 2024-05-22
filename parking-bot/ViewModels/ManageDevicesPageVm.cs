using ParkingBot.Models.Bt;
using ParkingBot.Models.BT;
using ParkingBot.Services;

using System.Collections.ObjectModel;

namespace ParkingBot.ViewModels;

public class ManageDevicesPageVm : BaseVm
{
    private readonly VehicleBluetoothService _vbs;

    public ObservableCollection<CarBtDevice> RegisteredCars { get; } = [];
    public ObservableCollection<BtDevice> PairedDevices { get; } = [];

    public ManageDevicesPageVm(VehicleBluetoothService vbs)
    {
        _vbs = vbs;
    }

    protected override void ExecuteLoadModelCommand()
    {
        _vbs.GetPairedDevices().Aggregate(PairedDevices, (devices, device) =>
        {
            devices.Add(device);
            return devices;
        });
        _vbs.GetRegisteredDevices().Aggregate(RegisteredCars, (cars, car) =>
        {
            cars.Add(car);
            return cars;
        });
    }
}
