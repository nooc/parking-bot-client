using Microsoft.Maui.LifecycleEvents;

using ParkingBot.Core;

namespace ParkingBot;

public class LifecycleConfig : LifecycleConfigBase
{
    public void Config(ILifecycleBuilder builder)
    {
        builder.AddiOS(iOSConfig);
    }

    private void iOSConfig(IiOSLifecycleBuilder iOSBuilder)
    {
        iOSBuilder.WillTerminate(del => Terminating());
    }
}
