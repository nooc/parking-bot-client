using ParkingBot.Pages;

namespace ParkingBot;

public partial class App : Application
{
    public App(MainPage mainPage)
    {
        UserAppTheme = AppTheme.Unspecified;

        InitializeComponent();

        MainPage = new NavigationPage(mainPage);
    }
}
