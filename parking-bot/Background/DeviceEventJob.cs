using Microsoft.Extensions.Logging;

using ParkingBot.Services;

using Shiny;
using Shiny.Jobs;

namespace ParkingBot.Background;

/// <summary>
/// Run from MultiDelegate when bt peripheral is (dis)connected.
/// </summary>
/// <param name="logger"></param>
/// <param name="services"></param>
public class DeviceEventJob(ILogger<DeviceEventJob> logger, IServiceProvider services) : Job(logger)
{
    private readonly Lazy<GeoFencingService> _geo = services.GetLazyService<GeoFencingService>();
    private readonly Lazy<VehicleBluetoothService> _bt = services.GetLazyService<VehicleBluetoothService>();

    protected override async Task Run(CancellationToken cancelToken)
    {
        var args = JobInfo.Parameters ?? [];
        var state = args["state"];

        if (state == "connected")
        {
            // add device to connected
            _bt.Value.Connect(args["uuid"], args["name"]);
            // start geolocation if not started
            await _geo.Value.SetEnabled(true);
        }
        else if (state == "disconnected")
        {
            // remove device from connected and disable geoloc if no devices
            var remaining = _bt.Value.Disconnect(args["uuid"]);
            if (remaining == 0) await _geo.Value.SetEnabled(false);
        }
    }
}
