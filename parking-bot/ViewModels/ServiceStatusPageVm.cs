using Microsoft.Extensions.Logging;

using ParkingBot.Models.Parking;
using ParkingBot.Properties;
using ParkingBot.Services;

namespace ParkingBot.ViewModels;

public class ServiceStatusPageVm(ILogger<MainPageVm> logger, GeoFencingService _geo,
        //KioskParkingService _kiosk,
        ServiceHelperService _hlp,
        TollParkingService _toll)
        : BaseVm(logger)
{
    public bool IsActive
    {
        get => Preferences.Get(Values.SRV_IS_ACTIVE, false);
        set => SetActiveState(value);
    }
    public string RegionName { get; private set; } = string.Empty;
    public bool HasRegion { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string Availability { get; private set; } = string.Empty;
    public string StatusText { get; private set; } = string.Empty;
    public string UserGreeting { get; private set; } = string.Empty;

    protected async override void ExecuteLoadModelCommand()
    {
        UserGreeting = $"{Lang.hello} {Preferences.Get(Values.TOKEN_USER_KEY, string.Empty)}";

        // get closest active region
        ParkingSite? site = null;
        if (_geo.IsGeoFencing)
        {
            site = await _geo.GetActiveRegion();
        }
        if (site != null)
        {
            RegionName = "Region X";
            Description = "Region desc";
            Availability = "Availability";
            OnPropertyChanged(nameof(RegionName));
        }
        else
        {
            RegionName = string.Empty;
            HasRegion = false;
            Description = string.Empty;
            Availability = Lang.not_available;
        }
        OnPropertyChanged(nameof(UserGreeting));
        OnPropertyChanged(nameof(HasRegion));
        OnPropertyChanged(nameof(Description));
        OnPropertyChanged(nameof(Availability));

        //TODO: status text
        //if (_kiosk.OngoingParking is ParkingTicket kticket)
        //{
        //    StatusText = "Ongoing kiosk";
        //}
        //else
        if (_toll.OngoingParking is ParkingTicket tticket)
        {
            StatusText = "Ongoing toll";
        }
        else StatusText = Lang.no_active_parking;

        OnPropertyChanged(nameof(StatusText));
    }

    private async void SetActiveState(bool enable)
    {
        if (enable) _hlp.Start();
        else await _hlp.StopAll();

        OnPropertyChanged(nameof(IsActive));
    }

    internal void TickWhenVisible()
    {
        LoadModelCommand.Execute(this);
    }
}
