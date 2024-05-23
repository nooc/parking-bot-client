using Microsoft.Extensions.Logging;

using ParkingBot.Models.Parking;
using ParkingBot.Services;

using System.Collections.ObjectModel;

namespace ParkingBot.ViewModels;

public class HistoryPageVm : BaseVm
{
    private readonly KioskParkingService _kiosk;
    private readonly TollParkingService _toll;

    public ObservableCollection<ParkingTicket> History { get; } = [];

    public HistoryPageVm(ILogger<HistoryPageVm> logger, KioskParkingService kioskParkingService, TollParkingService tollParkingService)
        : base(logger)
    {
        _kiosk = kioskParkingService;
        _toll = tollParkingService;
    }

    protected override void ExecuteLoadModelCommand()
    {
        History.Clear();
        List<ParkingTicket> history = [];
        history.AddRange(_kiosk.History);
        history.AddRange(_toll.History);
        history.Sort((a, b) => a.Timestamp.CompareTo(b.Timestamp));
        history.ForEach(History.Add);
    }

    private void UpdateHistory()
    {
        History.Clear();
        var history = _kiosk.History.Concat(_toll.History).OrderByDescending(item => item.Timestamp);
        foreach (var item in history) History.Add(item);
    }
}
