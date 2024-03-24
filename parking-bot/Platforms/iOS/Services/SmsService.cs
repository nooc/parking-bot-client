namespace ParkingBot.Services;

public sealed class SmsService
{
    public Task<bool> SendMessage(string recipient, string message, string? tag = null)
    {
        //TODO: implement iOS sms
        throw new NotImplementedException();
    }
}
