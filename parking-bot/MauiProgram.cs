using CommunityToolkit.Maui;

using Microsoft.Extensions.Logging;

using ParkingBot.Factories;
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
            .AddJobs()
            // Clients
            .AddSingleton(GetHttpClient())
            // Settings
            .AddSingleton<ParkingSettingsFactoryService>()
            // Services
            .AddSingleton<KioskParkingService>()
            .AddSingleton<TollParkingService>()
            .AddSingleton<GeoFencingService>()
            .AddSingleton<ServiceHelperService>()
            .AddSingleton<SmsService>()
            // ViewModels
            .AddSingleton<_ServiceControlPageVm>()
            .AddSingleton<ParkingMapPageVm>()
            .AddSingleton<SettingsPageVm>()
            // Views
            .AddTransient<ParkingMapPage>()
            .AddTransient<ServiceControlPage>()
            .AddTransient<MainPage>()
            .AddTransient<SettingsPage>()
            .AddTransient<AboutPage>()
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
