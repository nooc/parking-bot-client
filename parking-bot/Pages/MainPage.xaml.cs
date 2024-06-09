using Microsoft.Extensions.Logging;

using ParkingBot.Services;
using ParkingBot.ViewModels;

namespace ParkingBot.Pages;

public partial class MainPage : TabbedPage
{
    private readonly ServiceHelperService Services;

    public MainPage(ILogger<MainPage> _logger, MainPageVm vm, ServiceHelperService services,
        ServiceStatusPage _status, MapPage _map, HistoryPage _history
        )
    {
        Services = services;

        InitializeComponent();

        Children.Add(_status);
        Children.Add(_map);
        Children.Add(_history);

        BindingContext = vm;
    }
}