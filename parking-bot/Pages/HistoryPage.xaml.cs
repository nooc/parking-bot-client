using ParkingBot.ViewModels;

namespace ParkingBot.Pages;

public partial class HistoryPage : ContentPage
{
    private readonly HistoryPageVm Vm;

    public HistoryPage(HistoryPageVm viewModel)
    {
        InitializeComponent();

        BindingContext = Vm = viewModel; ;
    }
}