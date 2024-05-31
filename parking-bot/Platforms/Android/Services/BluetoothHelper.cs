using Android.Bluetooth;

using ParkingBot.Models.Bt;

namespace ParkingBot.Services;

public class BluetoothHelper
{
    private BluetoothManager? Manager = null;
    private BluetoothAdapter? Adapter = null;

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
        return Adapter?.BondedDevices?.Select(d => new BtDevice(d.Address ?? string.Empty, d.Alias ?? string.Empty)).ToList() ?? [];
    }

    public ICollection<BtDevice> GetConnectedDevices()
    {
        Init();
        return Manager?.GetConnectedDevices(ProfileType.A2dp)?
            .Select(d => new BtDevice(d.Address ?? string.Empty, d.Alias ?? string.Empty)).ToList() ?? [];
    }
}
