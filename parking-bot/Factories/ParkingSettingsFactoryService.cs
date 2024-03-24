using ParkingBot.Models.Parking;

using System.Text.Json;

namespace ParkingBot.Factories;

public class ParkingSettingsFactoryService
{
    private ParkingSettings? _settings = null;

    public ParkingSettings Instance
    {
        get
        {
            if (_settings == null) Read();

            return _settings ?? new ParkingSettings
            {
                Kiosk = null,
                MaxGpsDistance = -1,
                MinGpsAccuracy = -1,
                RegionRadius = -1,
                Toll = null
            };
        }
    }

    private async void Read()
    {
        using (var stream = await FileSystem.OpenAppPackageFileAsync("parking.json"))
        {
            var opt = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            if (stream != null && JsonSerializer.Deserialize<ParkingSettings>(stream, opt) is ParkingSettings settings)
            {
                _settings = settings;
            }
        }
    }
}
