using Android.App;
using Android.Content;
using Android.Content.PM;

using ParkingBot.Services;

namespace ParkingBot;

[Activity(
    Theme = "@style/Maui.MainTheme.NoActionBar",
    MainLauncher = true,
    ConfigurationChanges =
    ConfigChanges.ScreenSize |
    ConfigChanges.Orientation |
    ConfigChanges.UiMode |
    ConfigChanges.ScreenLayout |
    ConfigChanges.SmallestScreenSize |
    ConfigChanges.Density)]
[IntentFilter(
    [Shiny.ShinyNotificationIntents.NotificationClickAction],
    Categories = ["android.intent.category.DEFAULT"])]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        if (requestCode == AuthService.CHOOSE_ACCOUNT)
        {
            var aName = data?.GetStringExtra("KEY_ACCOUNT_NAME");
            var aType = data?.GetStringExtra("KEY_ACCOUNT_TYPE");
            AuthService.SelectedAccount.OnNext(new AuthService.AccountNameAndType { Name = aName, Type = aType });
        }
        base.OnActivityResult(requestCode, resultCode, data);
    }
}
