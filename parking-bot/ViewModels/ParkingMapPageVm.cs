using Mapsui.Layers;
using Mapsui.Styles;

namespace ParkingBot.ViewModels;

public class ParkingMapPageVm : BaseVm
{
    private string _Zoom = string.Empty;
    private Mapsui.Map? _Map;

    public string Title => "Map";
    public string Zoom { get => _Zoom; set => SetProperty(ref _Zoom, value); }
    public Command LoadModelCommand { get; private set; }
    public ObservableMemoryLayer<Mapsui.UI.Maui.Pin> PinLayer { get; private set; }
    public Mapsui.Map? Map
    {
        get => _Map;
        internal set
        {
            _Map = value;
            if (_Map != null)
            {
                LocationLayer = new MyLocationLayer(_Map);
                _Map.Layers.Add(PinLayer);
                _Map.Layers.Add(LocationLayer);
            }
        }
    }
    public MyLocationLayer? LocationLayer { get; internal set; }

    public ParkingMapPageVm()
    {
        PinLayer = new ObservableMemoryLayer<Mapsui.UI.Maui.Pin>(p =>
        {
            return p.Feature;
        });
        PinLayer.Style = SymbolStyles.CreatePinStyle(symbolScale: 0.7);
        LoadModelCommand = new Command(ExecuteLoadModelCommand);

        //TODO: remove test
        PinLayer.ObservableCollection = new System.Collections.ObjectModel.ObservableCollection<Mapsui.UI.Maui.Pin>
        {
            new() {
                Position=new Mapsui.UI.Maui.Position(57.73637027332549, 12.031604859973248),
                Label = "Hello",
                Type = Mapsui.UI.Maui.PinType.Pin,
                IsVisible=true,
            }
        };
    }
    private void ExecuteLoadModelCommand()
    {
        IsBusy = true;
        try
        {

        }
        finally
        {
            IsBusy = false;
        }
    }
}
