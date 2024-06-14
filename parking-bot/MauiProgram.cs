using CommunityToolkit.Maui;

using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

using ParkingBot.Background;
using ParkingBot.Handlers;
using ParkingBot.Pages;
using ParkingBot.Services;
using ParkingBot.ViewModels;

using Shiny;

using SkiaSharp.Views.Maui.Controls.Hosting;


namespace ParkingBot;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        var lifecycle = new LifecycleConfig();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseShiny()
            .UseSkiaSharp(true)
            .ConfigureFonts(fonts =>
            {
                fonts
                .AddFont("OpenSans-Regular.ttf", "OpenSansRegular")
                .AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .ConfigureLifecycleEvents(lifecycle.Config)
            .Services
                .AddSingleton<ServiceData>()
                .AddPush<NotificationDelegate>()
                .AddNotifications<NotificationDelegate>()
                .AddGps<LocationDelegate>()
                .AddGeofencing<LocationDelegate>()
                .AddJobs()
                // Clients
                .AddSingleton<Http.HttpClientExt>()
                .AddSingleton<Http.HttpClientInt>()
                // Services
                .AddSingleton<AppService>()
                .AddSingleton<GothenburgOpenDataService>()
                .AddSingleton<VehicleBluetoothService>()
                .AddSingleton<BluetoothHelper>()
                .AddSingleton<TollParkingService>()
                .AddSingleton<GeoFencingService>()
                .AddSingleton<ServiceHelperService>()
                .AddSingleton<SmsService>()
                // ViewModels
                .AddSingleton<MainPageVm>()
                .AddSingleton<ServiceStatusPageVm>()
                .AddSingleton<MapPageVm>()
                .AddSingleton<SettingsPageVm>()
                .AddSingleton<HistoryPageVm>()
                .AddSingleton<ManageDevicesPageVm>()
                // Views
                .AddTransient<MapPage>()
                .AddTransient<ServiceStatusPage>()
                .AddTransient<HistoryPage>()
                .AddTransient<SettingsPage>()
                .AddTransient<AboutPage>()
                .AddTransient<MainPage>()
                .AddTransient<ManageDevicesPage>()
                // Startup
                .AddShinyService<PostInjetStartupService>()
        ;

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();
        lifecycle.App = app;
        return app;
    }
}
