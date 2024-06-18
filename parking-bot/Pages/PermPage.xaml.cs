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

    protected override void OnAppearing()
    {
        ContinueOnPermissionSuccess();
    }

    private Task ContinueOnPermissionSuccess()
    {
        try
        {
            Task.WaitAll(
                DoPerm<Permissions.LocationAlways>(),
                DoPerm<Permissions.Bluetooth>(),
                DoPerm<Permissions.Battery>(),
                DoPerm<Permissions.Sms>(),
                DoPerm<Permissions.PostNotifications>()
                );

            if (Application.Current != null)
            {
                Application.Current.MainPage = new NavigationPage(Main);
            }
        }
        catch (ApplicationException)
        {
            PermAtivity.IsRunning = false;
            PermButton.IsVisible = true;
            return DisplayAlert(Properties.Lang.error, Properties.Lang.permission_error_msg, "Ok");
        }
        return Task.CompletedTask;
    }

    private void PermButton_Clicked(object sender, EventArgs e)
    {
        PermAtivity.IsRunning = true;
        PermButton.IsVisible = false;
        ContinueOnPermissionSuccess();
    }
}