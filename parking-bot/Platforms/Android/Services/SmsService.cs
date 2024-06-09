using Android.App;
using Android.Content;
using Android.Telephony;

namespace ParkingBot.Services;

public sealed class SmsService
{
    public readonly int SENT_CODE = 123;
    private readonly SmsManager? smsManager;

    public SmsService()
    {
        smsManager = (SmsManager?)Platform.AppContext.GetSystemService(Java.Lang.Class.FromType(typeof(SmsManager)));
    }

    public Task<bool> SendMessage(string recipient, string message, string? tag = null)
    {
        if (smsManager != null)
        {
            var intent = new Intent();
            if (tag != null) intent.Extras?.PutString("tag", tag);
            var sentIntent = PendingIntent.GetBroadcast(Android.App.Application.Context, SENT_CODE, intent, PendingIntentFlags.OneShot);

#if ANDROID28_0_OR_GREATER
            smsManager.SendTextMessageWithoutPersisting(recipient, null, message, sentIntent, null);
#else
            throw new NotSupportedException();
#endif

            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}
