using ParkingBot.ViewModels;

namespace ParkingBot.Pages;

public partial class ServiceControlPage : ContentPage
{
    private readonly ServiceControlPageVm ViewModel;
    private readonly IDispatcherTimer Timer;

    public ServiceControlPage()
    {
        var wm = Application.Current?.Handler.MauiContext?.Services.GetService<ServiceControlPageVm>();
        if (wm != null) ViewModel = wm;
        else throw new NullReferenceException("ServiceControlPageVm is null.");

        InitializeComponent();

        BindingContext = ViewModel;
        Timer = Dispatcher.CreateTimer();
        Timer.Interval = TimeSpan.FromSeconds(30);
        Timer.Tick += Timer_Tick;
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        ViewModel.TickWhenVisible();
    }

    protected override void OnAppearing()
    {
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

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        ViewModel.LoadModelCommand.Execute(this);
    }
}
