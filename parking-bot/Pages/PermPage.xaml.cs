namespace ParkingBot.Pages;

public partial class PermPage : ContentPage
{
    private readonly Page Main;
    public PermPage(Page main)
    {
        Main = main;

        InitializeComponent();
    }

    private async Task DoPerm<T>() where T : Permissions.BasePermission, new()
    {
        try
        {
            var result = await Permissions.CheckStatusAsync<T>();
            if (result != PermissionStatus.Granted)
            {
                result = await Permissions.RequestAsync<T>();
            }
            if (result != PermissionStatus.Granted)
            {
                throw new ApplicationException("Permissions not met.");
            }
        }
        catch (PermissionException)
        {
        }
    }

    protected async override void OnAppearing()
    {
        try
        {
            await DoPerm<Permissions.LocationAlways>();
            await DoPerm<Permissions.Bluetooth>();
            await DoPerm<Permissions.Battery>();
            await DoPerm<Permissions.Sms>();
            await DoPerm<Permissions.PostNotifications>();

            if (Application.Current is Application app && !(app.MainPage is NavigationPage))
            {
                app.MainPage = Main;
            }
        }
        catch (ApplicationException)
        {
            Application.Current?.Quit();
        }
    }
}