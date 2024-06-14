using Microsoft.Maui.LifecycleEvents;

using ParkingBot.Core;

namespace ParkingBot;

internal class LifecycleConfig : LifecycleConfigBase
{
    public void Config(ILifecycleBuilder builder)
    {
        builder.AddAndroid(AndroidConfig);
    }

    private void AndroidConfig(IAndroidLifecycleBuilder androidBuilder)
    {
        androidBuilder.OnDestroy(del => Terminating());
    }
}
