namespace ParkingBot.Services;

public class UserAuthService
{
    private readonly HttpClient _ht;

    public UserAuthService(HttpClient ht)
    {
        _ht = ht;
        // Properties.Security.API_ID;
    }

    public Task<bool> AuthenticateAsync()
    {
        throw new NotImplementedException();
    }
}
