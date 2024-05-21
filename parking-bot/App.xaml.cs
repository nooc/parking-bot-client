using ParkingBot.Pages;
using ParkingBot.Services;

namespace ParkingBot;

public partial class App : Application
{
    private readonly UserAuthService Auth;
    public App(MainPage mainPage, UserAuthService auth)
    {
        Auth = auth;
        UserAppTheme = AppTheme.Unspecified;

        InitializeComponent();

        MainPage = new NavigationPage(mainPage);
    }

    protected override void OnStart()
    {
        base.OnStart();

        //TODO: auth
        //await Auth.AuthenticateAsync();
    }
}
