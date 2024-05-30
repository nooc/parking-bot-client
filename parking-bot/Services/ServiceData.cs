using ParkingBot.Models;
using ParkingBot.Models.Parking;

namespace ParkingBot.Services;

public class ServiceData
{
    /// <summary>
    /// Remote synced settings.
    /// </summary>
    public Api.PbData? Settings { get; set; } = null;

    /// <summary>
    /// Dictionary of identifier/region.
    /// </summary>
    public Dictionary<string, ParkingSite> ParkingSites { get; } = [];

    /// <summary>
    /// Dictionary of license-plate/ongoing-parking-ticket.
    /// </summary>
    public Dictionary<string, ParkingTicket> VehicleParkings { get; } = [];


}
