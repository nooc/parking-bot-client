using Microsoft.Maui.LifecycleEvents;

using ParkingBot.Core;

namespace ParkingBot;

internal class LifecycleConfig : LifecycleConfigBase
{
    public static void Config(ILifecycleBuilder builder)
    {
        builder.AddiOS(iOSConfig);
    }

    private static void iOSConfig(IiOSLifecycleBuilder iOSBuilder)
    {
        iOSBuilder.WillTerminate(del => Terminating());
    }
}
