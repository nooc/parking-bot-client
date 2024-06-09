using Android.Bluetooth;

using ParkingBot.Exceptions;
using ParkingBot.Models.Bt;
using ParkingBot.Properties;

namespace ParkingBot.Services;

public class BluetoothHelper
{
    private BluetoothManager? Manager = null;
    private BluetoothAdapter? Adapter = null;

    public static readonly List<BtDevice> ConnectedMac = [];

    private void Init()
    {
        if (Adapter == null)
        {
            var btMgrObj = Platform.AppContext.GetSystemService(Android.Content.Context.BluetoothService);
            if (btMgrObj is BluetoothManager btMgr && btMgr.Adapter is BluetoothAdapter adapter)
            {
                Manager = btMgr;
                Adapter = adapter;
            }
        }
    }

    public ICollection<BtDevice> GetPairedDevices()
    {
        Init();
        return Adapter?.BondedDevices?.Select(d => new BtDevice(d.Address ?? string.Empty, d.Name ?? string.Empty)).ToList() ?? [];
    }

    public ICollection<BtDevice> GetConnectedDevices()
    {
        Init();
        return ConnectedMac;
    }

    public async Task RequestPermissions()
    {
        PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Bluetooth>();
        }
        if (status != PermissionStatus.Granted)
        {
            throw new ApplicationPermissionError("Bluetooth", Lang.permission_error_msg);
        }
    }
}
