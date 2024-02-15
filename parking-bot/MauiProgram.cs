using CommunityToolkit.Maui;

using Shiny;

using ParkingBot.Factories;
using ParkingBot.Handlers;
using ParkingBot.Pages;
using ParkingBot.Services;
using ParkingBot.ViewModels;

using Microsoft.Extensions.Logging;


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
            .AddGeofencing<MultiDelegate>()
            .AddJobs()
            // Clients
            .AddSingleton(GetHttpClient())
            // Settings
            .AddSingleton<ParkingSettingsFactoryService>()
            // Services
            .AddSingleton<KioskParkingService>()
            .AddSingleton<SMSParkingService>()
            .AddSingleton<GeoFencingService>()
            .AddSingleton<ServiceHelperService>()
            .AddSingleton<SmsService>()
            // ViewModels
            .AddSingleton<MainPageVm>()
            .AddSingleton<SettingsPageVm>()
            // Views
            .AddTransient<TokenPage>()
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
