using Microsoft.Extensions.Logging;

using ParkingBot.Models.Bt;
using ParkingBot.Properties;
using ParkingBot.Services;
using ParkingBot.Util;

using System.Collections.ObjectModel;

namespace ParkingBot.ViewModels;

public class ManageDevicesPageVm : BaseVm
{
    private readonly VehicleBluetoothService _bt;

    public ObservableCollection<CarBtDevice> RegisteredCars { get; } = [];
    public ObservableCollection<BtDevice> PairedDevices { get; } = [];

    public Command RegisterDevice { get; }
    public Command UnregisterDevice { get; }

    public ManageDevicesPageVm(ILogger<ManageDevicesPageVm> logger, VehicleBluetoothService vbs) : base(logger)
    {
        _bt = vbs;
        RegisterDevice = new Command<BtDevice>(ExecuteRegisterDevice);
        UnregisterDevice = new Command<CarBtDevice>(ExecuteUnregisterDevice);
    }

    protected override async void ExecuteLoadModelCommand()
    {
        PairedDevices.Clear();
        RegisteredCars.Clear();

        _bt.GetPairedDevices().Aggregate(PairedDevices, (devices, device) =>
        {
            devices.Add(device);
            return devices;
        });
        var regCars = await _bt.GetRegisteredDevices();
        regCars?.Aggregate(RegisteredCars, (cars, car) =>
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
                    keyboard: Values.KEYBOARD_CAPITAL,
                    placeholder: "ABC123");
                if (reg == null) break;
                else if (IsValidLicensePlate(reg))
                {
                    RegisteredCars.Add(new(reg, device));
                    PairedDevices.Remove(device);
                    break;
                }
            }
        }
    }
    private static bool IsValidLicensePlate(string licensePlate)
    {
        return RegexUtils.LicensePlateRegex().IsMatch(licensePlate.Trim().ToUpper());
    }

    private void ExecuteUnregisterDevice(CarBtDevice device)
    {
        RegisteredCars.Remove(device);
        PairedDevices.Add(new(device.DeviceId, device.DeviceName));
    }
}
