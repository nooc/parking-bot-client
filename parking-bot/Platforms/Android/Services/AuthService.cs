using Android.Accounts;
using Android.OS;

using Java.Util.Concurrent;

using System.Reactive.Subjects;

namespace ParkingBot.Services;


public class AuthService
{
    private static readonly string KEY_AUTHTOKEN = "KEY_AUTHTOKEN";
    public class AccountNameAndType
    {
        public string? Name { get; set; } = null;
        public string? Type { get; set; } = null;
    }

    public static readonly int CHOOSE_ACCOUNT = 1010;

    private readonly AccountManager? accountManager;

    internal static readonly ISubject<AccountNameAndType> SelectedAccount = new Subject<AccountNameAndType>();

    public static string? Token { get; private set; } = null;

    public AuthService()
    {
        accountManager = AccountManager.Get(Android.App.Application.Context.ApplicationContext);
        SelectedAccount.Subscribe(OnAccountSelected);
    }

    private async void OnAccountSelected(AccountNameAndType selected)
    {
        if (accountManager != null && selected.Name != null)
        {
            foreach (var acc in accountManager.GetAccounts())
            {
                if (acc.Name?.Equals(selected.Name) ?? false)
                {
                    var future = accountManager.GetAuthToken(acc, "JWT", null, null, null, null);
                    if (future != null)
                    {
                        var result = await future.GetResultAsync(30, TimeUnit.Seconds);

                        if (result is Bundle bundle && bundle.ContainsKey(KEY_AUTHTOKEN))
                        {
                            Token = bundle.GetString(KEY_AUTHTOKEN);

                        }
                    }
                }
            }
        }
    }

    public static void ChooseAccount()
    {
        var intent = AccountManager.NewChooseAccountIntent(null, null, ["com.google"], null, null, null, null);
        Platform.CurrentActivity?.StartActivityForResult(intent, CHOOSE_ACCOUNT);
    }
}
