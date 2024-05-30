using ParkingBot.ViewModels;

namespace ParkingBot.Pages;

public partial class ServiceStatusPage : ContentPage
{
    private readonly ServiceStatusPageVm Vm;
    private readonly IDispatcherTimer Timer;

    public ServiceStatusPage(ServiceStatusPageVm viewMovel)
    {
        Timer = Dispatcher.CreateTimer();

        InitializeComponent();

        Timer.Interval = TimeSpan.FromSeconds(30);
        Timer.Tick += Timer_Tick;
        BindingContext = Vm = viewMovel;
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        Vm.TickWhenVisible();
    }

    protected override void OnAppearing()
    {
        Vm.LoadModelCommand.Execute(this);
        Timer.Start();
    }

    protected override void OnDisappearing()
    {
        Timer.Stop();
    }

    private async void Settings_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(Handler?.MauiContext?.Services.GetService<SettingsPage>());
        ;
    }

    private async void About_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(Handler?.MauiContext?.Services.GetService<AboutPage>());
        ;
    }

    private async void ManageDevices_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(Handler?.MauiContext?.Services.GetService<ManageDevicesPage>());
    }
}
