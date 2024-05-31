using ParkingBot.Properties;
using ParkingBot.ViewModels;

namespace ParkingBot.Pages;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsPageVm Vm;

    public SettingsPage(SettingsPageVm viewModel)
    {
        InitializeComponent();

        BindingContext = Vm = viewModel;
    }

    /// <summary>
    /// Add/get kiosk info by id from kiosk endpoint.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void AddKiosk_Clicked(object sender, EventArgs e)
    {
        var id = await DisplayPromptAsync(
                    title: "Kiosk Id", $"Add kiosk id",
                    placeholder: "00000000-aaaa-bbbb-1111-cccccccccccc");
        ;
        if ((id = id?.Trim().ToLower() ?? string.Empty) == string.Empty) return;
        if (Vm.HasKiosk(id))
        {
            await DisplayAlert(Lang.exists, $"Kiosk with id {id} already exists", "Ok");
        }
        else
        {
            try
            {
                Vm.AddKiosk(id);
            }
            catch (Exception ex)
            {
                await DisplayAlert(Lang.error, ex.Message, "Ok");
            }
        }
    }

    protected override void OnAppearing()
    {
        Vm.LoadModelCommand.Execute(this);
    }
}
