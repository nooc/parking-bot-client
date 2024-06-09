using Microsoft.Extensions.Logging;

using ParkingBot.Services;

namespace ParkingBot.ViewModels;

public class MainPageVm(ILogger<MainPageVm> logger, ServiceHelperService _hlp) : BaseVm(logger)
{
    protected override void ExecuteLoadModelCommand(Page page)
    {
        _hlp.GetSettings(true);
    }
}
