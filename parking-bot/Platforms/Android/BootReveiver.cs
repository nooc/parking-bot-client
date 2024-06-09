using Android.App;
using Android.Content;

namespace ParkingBot.Platforms.Android;

[BroadcastReceiver(Label = "BootReceiver", DirectBootAware = true, Enabled = true, Exported = true)]
[IntentFilter([Intent.ActionBootCompleted])]
class BootReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? _context, Intent? _intent)
    {
        if (_context is Context context && _intent is Intent intent)
        {
            //launch our activity
            if (intent.Action == "android.intent.action.BOOT_COMPLETED")
            {
                Intent start = new Intent(context, typeof(MainActivity));
                start.SetFlags(ActivityFlags.NewTask);
                context.StartActivity(start);
            }
        }
    }
}
