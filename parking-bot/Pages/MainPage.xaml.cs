using ParkingBot.Services;
using ParkingBot.ViewModels;

namespace ParkingBot.Pages;

public partial class MainPage : TabbedPage
{
    private readonly ServiceHelperService Services;

    public MainPage(MainPageVm vm, ServiceStatusPage ssp, ParkingMapPage pmp, HistoryPage hp, ServiceHelperService services)
    {
        Services = services;

        InitializeComponent();

        BindingContext = vm;

        Children.Add(ssp);
        Children.Add(pmp);
        Children.Add(hp);
    }
}