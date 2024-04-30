namespace ParkingBot.Services;

public interface IAuthService
{
    Task<bool> AuthenticateAsync();
}
