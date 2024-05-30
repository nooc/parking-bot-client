using Microsoft.Extensions.Logging;

using ParkingBot.Models.Parking;
using ParkingBot.Services;

using System.Collections.ObjectModel;

namespace ParkingBot.ViewModels;

public class HistoryPageVm(ILogger<HistoryPageVm> logger,
    //KioskParkingService _kiosk,
    TollParkingService _toll)
    : BaseVm(logger)
{

    public ObservableCollection<ParkingTicket> History { get; } = [];

    protected override void ExecuteLoadModelCommand()
    {
        History.Clear();
        List<ParkingTicket> history = [];
        //history.AddRange(_kiosk.History);
        history.AddRange(_toll.History);
        history.Sort((a, b) => a.Stop?.CompareTo(b.Stop ?? DateTime.MaxValue) ?? 0);
        history.ForEach(History.Add);
    }
}
