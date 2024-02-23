using ParkingBot.ViewModels;

namespace ParkingBot.Pages;

public partial class ParkingMapPage : ContentPage
{
    private readonly ParkingMapPageVm ViewModel;

    public ParkingMapPage()
    {
        var wm = Application.Current?.Handler.MauiContext?.Services.GetService<ParkingMapPageVm>();
        if (wm != null) ViewModel = wm;
        else throw new NullReferenceException("ParkingMapPageVm is null.");

        InitializeComponent();

        MapCtrl.Map?.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        ViewModel.LoadModelCommand.Execute(this);
    }
}