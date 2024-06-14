using Shiny.Notifications;
using Shiny.Push;

namespace ParkingBot.Background;

internal class NotificationDelegate : IPushDelegate, INotificationDelegate
{
    //
    // IPushDelegate
    //

    public Task OnEntry(PushNotification notification)
    {
        throw new NotImplementedException();
    }

    public Task OnNewToken(string token)
    {
        throw new NotImplementedException();
    }

    public Task OnReceived(PushNotification notification)
    {
        throw new NotImplementedException();
    }

    public Task OnUnRegistered(string token)
    {
        throw new NotImplementedException();
    }

    //
    // INotificationDelegate
    //

    public Task OnEntry(NotificationResponse response)
    {
        throw new NotImplementedException();
    }
}
