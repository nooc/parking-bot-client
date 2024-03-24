using Android.App;
using Android.Content;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingBot.Services;

public class SmsSentReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if(intent!=null && intent.Extras!=null && intent.Extras.GetCharSequence("tag") is string tag)
        {
            if ((tag.Equals("start") && ResultCode != Result.Ok) || tag.Equals("stop")&& ResultCode == Result.Ok)
            {
                // remove ongoing

            }
        }
    }
}
