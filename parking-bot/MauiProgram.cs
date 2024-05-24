using CommunityToolkit.Maui;

using Microsoft.Extensions.Logging;

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
            });

        IServiceCollection isc = builder.Services;

        // Shiny
        isc.AddGps<MultiDelegate>()
            .AddGeofencing<MultiDelegate>()
            .AddNotifications<MultiDelegate>()
            .AddBluetoothLE<MultiDelegate>()
            .AddJobs()
            // Clients
            .AddSingleton(GetHttpClient())
            // Services
            .AddSingleton<UserAuthService>()
            .AddSingleton<VehicleBluetoothService>()
            .AddSingleton<KioskParkingService>()
            .AddSingleton<TollParkingService>()
            .AddSingleton<GeoFencingService>()
            .AddSingleton<ServiceHelperService>()
            .AddSingleton<SmsService>()
            // ViewModels
            .AddSingleton<MainPageVm>()
            .AddSingleton<ServiceStatusPageVm>()
            .AddSingleton<ParkingMapPageVm>()
            .AddSingleton<SettingsPageVm>()
            .AddSingleton<HistoryPageVm>()
            .AddSingleton<ManageDevicesPageVm>()
            // Views
            .AddTransient<ParkingMapPage>()
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

    private static HttpClient GetHttpClient()
    {
        return new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(5)
        };
    }
}
