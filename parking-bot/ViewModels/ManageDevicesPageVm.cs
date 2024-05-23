using Microsoft.Extensions.Logging;

using ParkingBot.Models.Bt;
using ParkingBot.Services;
using ParkingBot.Util;

using System.Collections.ObjectModel;

namespace ParkingBot.ViewModels;

public class ManageDevicesPageVm : BaseVm
{
    private readonly VehicleBluetoothService _vbs;

    //TODO: Remove hardcoded
    public ObservableCollection<CarBtDevice> RegisteredCars { get; } = [
        new ("ABC123", new("2f4d809e-7ed4-4033-9cae-bba1db5ca4f6", "My Volvo device")),
        new ("ZYX987", new("1f4d809e-7ed4-4033-00ae-bba1db5ca4f9", "My Audi device"))
        ];
    public ObservableCollection<BtDevice> PairedDevices { get; } = [
        new("000d809e-7ed4-4033-0000-bba1db5ca4f6", "My Sisters car"),
        new("111d809e-7ed4-4033-1111-bba1db5ca4f9", "Some rental car")
        ];

    public Command RegisterDevice { get; }
    public Command UnregisterDevice { get; }

    public ManageDevicesPageVm(ILogger<ManageDevicesPageVm> logger, VehicleBluetoothService vbs) : base(logger)
    {
        _vbs = vbs;
        RegisterDevice = new Command<BtDevice>(ExecuteRegisterDevice);
        UnregisterDevice = new Command<CarBtDevice>(ExecuteUnregisterDevice);
    }

    protected override void ExecuteLoadModelCommand()
    {
        PairedDevices.Clear();
        RegisteredCars.Clear();

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

    private async void ExecuteRegisterDevice(BtDevice device)
    {
        bool prompt = true;
        if (Application.Current?.MainPage is Page page)
        {
            while (prompt)
            {
                var reg = await page.DisplayPromptAsync(
                    title: "Set plate number", $"Set license plate for {device.DeviceName}.",
                    keyboard: VehicleUtils.LicensePlateKeyboard,
                    placeholder: "ABC123");
                if (reg == null) break;
                else if (VehicleUtils.IsValidLicensePlate(reg))
                {
                    RegisteredCars.Add(new(reg, device));
                    PairedDevices.Remove(device);
                    break;
                }
            }
        }
    }

    private void ExecuteUnregisterDevice(CarBtDevice device)
    {
        RegisteredCars.Remove(device);
        PairedDevices.Add(new(device.DeviceId, device.DeviceName));
    }
}
