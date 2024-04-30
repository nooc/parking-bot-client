using Google.Apis.PlayIntegrity.v1;

using ParkingBot.Properties;

using Data = Google.Apis.PlayIntegrity.v1.Data;

namespace ParkingBot.Services;

internal class AuthServicePlayIntegrity : IAuthService
{
    private readonly V1Resource resource;
    public AuthServicePlayIntegrity()
    {
        var playIntegrityService = new PlayIntegrityService(
            new Google.Apis.Services.BaseClientService.Initializer
            {
                ApiKey = Sec.GOOGLE_API_KEY,
                ApplicationName = Values.USER_AGENT
            }
        );
        resource = playIntegrityService.V1;
    }

    public async Task<bool> AuthenticateAsync()
    {

        //TODO: where to get integrity token
        var req = new Data.DecodeIntegrityTokenRequest { }
        var resp = await req.ExecuteAsync();
        return true;
    }
}
