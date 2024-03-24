using ParkingBot.ViewModels;

namespace ParkingBot.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsPageVm viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}