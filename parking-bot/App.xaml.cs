using ParkingBot.Pages;
using ParkingBot.Properties;

namespace ParkingBot;

public partial class App : Application
{
    public App(MainPage mainPage, TokenPage tokenPage)
    {
        UserAppTheme = AppTheme.Unspecified;

        InitializeComponent();

        var verified = Preferences.Get(Values.TOKEN_VERIFIED_KEY, false);
        MainPage = verified ? new NavigationPage(mainPage) : tokenPage;
    }
}
