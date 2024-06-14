using ParkingBot.Background;

using Shiny.Jobs;

namespace ParkingBot.Core;

/// <summary>
/// Handle common lifecycle events.
/// </summary>
public abstract class LifecycleConfigBase
{
    public MauiApp? App { get; set; }

    /// <summary>
    /// Called when app terminates.
    /// </summary>
    protected void Terminating()
    {
        // End parking if terminating app as we can't manage parking with
        // terminated app.
        // TODO: Possible future scenario is to start external wakeup, for example, push notifications
        // when app terminates and there are active parkings.
        // (Then when app starts again, disable push notifiications)

        var jobs = App?.Services.GetService<IJobManager>();
        jobs?.Register(new JobInfo(nameof(TerminationJob), typeof(TerminationJob), true));
    }
}
