namespace ParkingBot.Core;

internal class LifecycleConfigBase
{
    protected static void Terminating()
    {
        // TODO: End parking if terminating app for now.
        // In the future have a background service
        // and terminate there if the service is terminated.
    }
}
