using ParkingBot.Pages;
using ParkingBot.Services;

namespace ParkingBot;

public partial class App : Application
{
    private readonly AppService Api;

    public App(MainPage mainPage, AppService api)
    {
        Api = api;
        UserAppTheme = AppTheme.Unspecified;

        InitializeComponent();

        MainPage = new PermPage(mainPage);
    }
}
