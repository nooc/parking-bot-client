using ParkingBot.Models.Parking;

namespace ParkingBot.Services;

public class ServiceData
{
    public Dictionary<string, ParkingSite> ParkingSites { get; } = [];
}
