namespace ParkingBot.Pages;

public partial class MainPage : TabbedPage
{
    public MainPage(ServiceControlPage scp, ParkingMapPage pmp, HistoryPage hp)
    {
        InitializeComponent();

        Children.Add(scp);
        Children.Add(pmp);
        Children.Add(hp);
    }

    protected override void OnAppearing()
    {

    }
}