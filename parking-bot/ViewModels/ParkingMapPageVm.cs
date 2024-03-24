namespace ParkingBot.ViewModels;

public class ParkingMapPageVm : BaseVm
{
    private string _Zoom = string.Empty;

    public string Title => "Map";
    public string Zoom { get => _Zoom; set => SetProperty(ref _Zoom, value); }
    public Command LoadModelCommand { get; private set; }

    public ParkingMapPageVm()
    {
        LoadModelCommand = new Command(ExecuteLoadModelCommand);
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
