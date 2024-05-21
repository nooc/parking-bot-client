using ParkingBot.Services;

namespace ParkingBot.Pages;

public partial class MainPage : TabbedPage
{
    private readonly ServiceHelperService Services;

    public MainPage(ServiceStatusPage ssp, ParkingMapPage pmp, HistoryPage hp, ServiceHelperService services)
    {
        Services = services;

        InitializeComponent();

        Children.Add(ssp);
        Children.Add(pmp);
        Children.Add(hp);
    }

    protected override async void OnAppearing()
    {
        await Services.RequestAccess();
    }
}