using Microsoft.Extensions.Logging;

namespace ParkingBot.ViewModels;

public class MainPageVm : BaseVm
{
    public MainPageVm(ILogger<MainPageVm> logger) : base(logger)
    {
    }

    protected override void ExecuteLoadModelCommand()
    {
    }
}
