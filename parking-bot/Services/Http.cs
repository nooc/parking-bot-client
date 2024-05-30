using ParkingBot.Properties;

using System.Net.Http.Headers;

namespace ParkingBot.Services;

public sealed class Http
{
    private static readonly ProductInfoHeaderValue AGENT = new(Values.USER_AGENT_NAME, Values.USER_AGENT_VER);
    public class HttpClientInt : HttpClient
    {
        public HttpClientInt() : base()
        {
            BaseAddress = new Uri(Values.APP_SERVICE_URI);
            DefaultRequestHeaders.UserAgent.Add(AGENT);
        }

        public string? BearerToken
        {
            get => DefaultRequestHeaders.Authorization?.Parameter;
            set
            {
                if (value != null)
                {
                    DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", value);
                }
                else DefaultRequestHeaders.Authorization = null;
            }
        }
    }

    public class HttpClientExt : HttpClient
    {
        public HttpClientExt() : base()
        {
            DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            DefaultRequestHeaders.UserAgent.Add(AGENT);
        }
    }
}
