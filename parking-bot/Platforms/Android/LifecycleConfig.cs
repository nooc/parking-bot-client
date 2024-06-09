using Microsoft.Maui.LifecycleEvents;

using ParkingBot.Core;

namespace ParkingBot;

internal class LifecycleConfig : LifecycleConfigBase
{
    public static void Config(ILifecycleBuilder builder)
    {
        builder.AddAndroid(AndroidConfig);
    }

    private static void AndroidConfig(IAndroidLifecycleBuilder androidBuilder)
    {
        androidBuilder.OnDestroy(del => Terminating());
    }
}
