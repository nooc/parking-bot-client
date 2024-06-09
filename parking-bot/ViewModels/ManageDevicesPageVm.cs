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
    private readonly ServiceHelperService _hlp;

    public ObservableCollection<CarBtDevice> RegisteredCars { get; } = [];
    public ObservableCollection<BtDevice> PairedDevices { get; } = [];

    public Command RegisterDevice { get; }
    public Command UnregisterDevice { get; }

    public ManageDevicesPageVm(ILogger<ManageDevicesPageVm> logger, VehicleBluetoothService vbs, ServiceHelperService hlp) : base(logger)
    {
        _bt = vbs;
        _hlp = hlp;
        RegisterDevice = new Command<BtDevice>(ExecuteRegisterDevice);
        UnregisterDevice = new Command<CarBtDevice>(ExecuteUnregisterDevice);
    }

    protected override void ExecuteLoadModelCommand(Page page)
    {
        PairedDevices.Clear();
        RegisteredCars.Clear();
        List<string> regUuid = [];
        var regCars = _bt.GetRegisteredDevices();
        foreach (var car in regCars)
        {
            regUuid.Add(car.DeviceId);
            RegisteredCars.Add(car);
        }
        var devs = _bt.GetPairedDevices();
        foreach (var dev in devs)
        {
            if (!regUuid.Contains(dev.DeviceId))
            {
                // do not add registered to paired list
                PairedDevices.Add(dev);
            }
        }
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
                    _bt.RegisterCar(new CarBtDevice(reg, device));
                    LoadModelCommand.Execute(null);
                    return;
                }
            }
        }
    }
    /// <summary>
    /// Valid format and not conflicting.
    /// </summary>
    /// <param name="licensePlate"></param>
    /// <returns></returns>
    private bool IsValidLicensePlate(string licensePlate)
    {
        var plate = licensePlate.Trim().ToUpper();
        var conflict = RegisteredCars.Where(item => item.RegNumber == plate).FirstOrDefault();
        if (conflict != null) return false;
        return RegexUtils.LicensePlateRegex().IsMatch(plate);
    }

    private void ExecuteUnregisterDevice(CarBtDevice device)
    {
        _bt.RemoveCar(device.DeviceId);
        LoadModelCommand.Execute(this);
    }
}
