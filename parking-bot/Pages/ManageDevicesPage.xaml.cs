using ParkingBot.ViewModels;

namespace ParkingBot.Pages;

public partial class ManageDevicesPage : ContentPage
{
    private readonly ManageDevicesPageVm Vm;

    public ManageDevicesPage(ManageDevicesPageVm viewModel)
    {
        InitializeComponent();

        BindingContext = Vm = viewModel;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Vm.LoadModelCommand.Execute(this);
    }
}