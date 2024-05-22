using Microsoft.Extensions.Logging;

using ParkingBot.Models.Parking;
using ParkingBot.Properties;
using ParkingBot.Services;

namespace ParkingBot.ViewModels;

public class ServiceStatusPageVm : BaseVm
{
    private readonly GeoFencingService _geo;
    private readonly KioskParkingService _kiosk;
    private readonly TollParkingService _toll;
    private readonly ILogger _logger;

    private ISiteInfo? Info = null;
    public bool IsActive
    {
        get => _geo.IsGeoFencing;
        set => SetActiveState(value);
    }
    public string RegionName { get; private set; }
    public bool HasRegion { get; private set; }
    public string Description { get; private set; }
    public string Availability { get; private set; }
    public string StatusText { get; private set; }
    public string UserGreeting { get; private set; }

    public ServiceStatusPageVm(ILogger<MainPageVm> logger, GeoFencingService geofencingService,
        KioskParkingService kioskParkingService, TollParkingService smsParkingService)
        : base()
    {
        _geo = geofencingService;
        _logger = logger;
        _kiosk = kioskParkingService;
        _toll = smsParkingService;

        HasRegion = false;
        RegionName = string.Empty;
        Description = string.Empty;
        Availability = string.Empty;
        StatusText = string.Empty;
        UserGreeting = string.Empty;
    }

    protected async override void ExecuteLoadModelCommand()
    {
        IsBusy = true;
        try
        {
            UserGreeting = $"{Lang.hello} {Preferences.Get(Values.TOKEN_USER_KEY, string.Empty)}";

            KioskSite? site;
            if (_geo.IsGeoFencing) site = await _geo.GetActiveRegion();
            else site = null;

            if (site != null)
            {
                if (Info != null && Info.SiteId == site.SiteId)
                    Info = await _kiosk.GetSiteInfoAsync(site);

                RegionName = Info?.SiteName ?? string.Empty;
                Description = Info?.SiteDescription ?? string.Empty;
                Availability = Info?.SiteAvailability ?? string.Empty;
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

            if (_kiosk.OngoingParking is ParkingTicket kticket)
            {
                StatusText = kticket.GetSummary();
            }
            else if (_toll.OngoingParking is ParkingTicket tticket)
            {
                StatusText = tticket.GetSummary();
            }
            else StatusText = Lang.no_active_parking;

            OnPropertyChanged(nameof(StatusText));
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, nameof(MainPageVm));
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void SetActiveState(bool value)
    {
        if (_geo != null)
        {
            await _geo.SetEnabled(value);
        }
        OnPropertyChanged(nameof(IsActive));
    }

    internal void TickWhenVisible()
    {
        LoadModelCommand.Execute(this);
    }
}
