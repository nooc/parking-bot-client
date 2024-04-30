using ParkingBot.Pages;
using ParkingBot.Services;

namespace ParkingBot;

public partial class App : Application
{
    private readonly IAuthService Auth;
    public App(MainPage mainPage, IAuthService auth)
    {
        Auth = auth;
        UserAppTheme = AppTheme.Unspecified;

        InitializeComponent();

        MainPage = new NavigationPage(mainPage);
    }

    protected override async void OnStart()
    {
        base.OnStart();

        await Auth.AuthenticateAsync();
    }
}
