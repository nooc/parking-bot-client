using Microsoft.Extensions.Logging;

namespace ParkingBot.Services;

public class PostInjetStartupService(ILogger<PostInjetStartupService> logger) : Shiny.IShinyStartupTask
{

    public void Start()
    {
        /*
         * Maybe add job from preferences when broadcast receiver gets notified of device state.. ?
         * 
        Dictionary<string, string> jobParams = new()
        {
            { "uuid", peripheral.Uuid },
            { "name", peripheral.Name??string.Empty }
        };

        if (peripheral.Status == ConnectionState.Connected)
        {
            jobParams.Add("state", "connected");
        }
        else if (peripheral.Status == ConnectionState.Disconnected)
        {
            jobParams.Add("state", "disconnected");
        }
        else return Task.CompletedTask;
        _jobs.Register(new JobInfo(nameof(DeviceEventJob), typeof(DeviceEventJob), false, jobParams));
        return Task.CompletedTask;
        */

    }
}
