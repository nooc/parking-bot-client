using Microsoft.Extensions.Logging;

using ParkingBot.Services;

namespace ParkingBot.ViewModels;

public class MainPageVm(ILogger<MainPageVm> logger, ServiceHelperService _hlp) : BaseVm(logger)
{
    protected override void ExecuteLoadModelCommand()
    {
        _hlp.GetSettings(true);
    }
}
