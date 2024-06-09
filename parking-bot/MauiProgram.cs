using CommunityToolkit.Maui;

using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

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
            .ConfigureLifecycleEvents(LifecycleConfig.Config);

        IServiceCollection isc = builder.Services;

        isc.AddSingleton<ServiceData>()
            // Shiny
            .AddGps<MultiDelegate>()
            .AddGeofencing<MultiDelegate>()
            .AddNotifications<MultiDelegate>()
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

        return builder.Build();
    }
}
