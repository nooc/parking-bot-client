using Android.Accounts;

namespace ParkingBot.Services;


public class AuthService
{
    public static readonly int CHOOSE_ACCOUNT = 1010;

    private readonly AccountManager? accountManager;


    public AuthService()
    {
        accountManager = AccountManager.Get(Android.App.Application.Context.ApplicationContext);
    }

    public async void GetAccount()
    {
        var intent = AccountManager.NewChooseAccountIntent(null, null, ["com.google"], null, null, null, null);
        Platform.CurrentActivity?.StartActivityForResult(intent, CHOOSE_ACCOUNT);
        await Task.Run(()-> {
            while ()
        });
    }
}
