namespace ParkingBot.Services;

public class UserAuthService
{
    //private readonly V1Resource resource;
    public UserAuthService()
    {/*
        var playIntegrityService = new PlayIntegrityService(
            new Google.Apis.Services.BaseClientService.Initializer
            {
                ApiKey = Sec.GOOGLE_API_KEY,
                ApplicationName = Values.USER_AGENT
            }
        );
        resource = playIntegrityService.V1;
        */
    }

    public Task<bool> AuthenticateAsync()
    {

        //TODO: where to get integrity token
        // var req = new Data.DecodeIntegrityTokenRequest { }
        // var resp = await req.ExecuteAsync();
        throw new NotImplementedException();
    }
}
