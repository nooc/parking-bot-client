
using Android.App;
using Android.Bluetooth;
using Android.Content;

using ParkingBot.Models.Bt;

namespace ParkingBot.Platforms.Android;

[BroadcastReceiver(Label = "DeviceConnectionReceiver", Enabled = true, Exported = true)]
[IntentFilter([BluetoothAdapter.ActionStateChanged])]
public class DeviceConnectionReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? _context, Intent? _intent)
    {
        if (_context is Context context && _intent is Intent intent)
        {
            if (intent.Action == BluetoothAdapter.ActionConnectionStateChanged)
            {
                var managerObj = context.GetSystemService(Context.BluetoothService);
                if (managerObj is BluetoothManager manager && manager.Adapter is BluetoothAdapter adapter)
                {
                    var bonded = adapter.BondedDevices;
                    if (bonded != null)
                    {
                        Services.BluetoothHelper.ConnectedMac.Clear();
                        foreach (var device in bonded)
                        {
                            if (manager.GetConnectionState(device, ProfileType.A2dp) == ProfileState.Connected && device.Address != null)
                            {
                                Services.BluetoothHelper.ConnectedMac.Add(new BtDevice(device.Address, device.Name ?? "???"));
                            }
                        }
                    }
                }
            }
        }
    }
}
