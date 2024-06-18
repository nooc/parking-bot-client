using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using ParkingBot.Models;
using ParkingBot.Properties;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ParkingBot.Services;

public class AppService(ILogger<AppService> logger, Http.HttpClientInt client)
{
    private static readonly SigningCredentials TokenCredentials = new SigningCredentials(
        new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Values.HS256_KEY)),
        SecurityAlgorithms.HmacSha256
        );
    private static readonly Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler TokenHandler = new()
    {
        SetDefaultTimesOnTokenCreation = false
    };
    private readonly TokenValidationParameters ValidationParams = new()
    {
        ValidIssuer = Values.JWT_ISS,
        ValidateLifetime = true,
        RequireExpirationTime = true,
        IssuerSigningKey = TokenCredentials.Key,
        RequireSignedTokens = true,
        RequireAudience = true,
        ValidAudience = Values.JWT_AUD
    };
    //private readonly JsonSerializerOptions JsonSerOpt = new() {
    //};
    private static readonly MediaTypeHeaderValue JsonType = new MediaTypeHeaderValue("application/json");

    private StringContent serialize<T>(object obj)
    {
        return new StringContent(
            JsonSerializer.Serialize(obj, typeof(T)),
            mediaType: JsonType
            );
    }

    private async Task<T?> Read<T>(HttpResponseMessage msg)
    {
        try
        {
            //logger.LogInformation(msg.RequestMessage?.ToString());
            return await msg.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<T>();
        }
        catch (Exception ex)
        {
            logger.LogError(msg.Headers.ToString(), ex);
            logger.LogCritical(ex, "Failed to read response.");
        }
        return default;
    }

    private async Task<T?> Get<T>(string url)
    {
        await InitUser();
        var result = await client.GetAsync(url);
        return await Read<T>(result);
    }
    private async Task<T?> Put<T, P>(string url, P data)
    {
        await InitUser();
        var content = data != null ? serialize<P>(data) : null;
        var result = await client.PutAsync(url, content);
        return await Read<T>(result);
    }
    private async Task<T?> Post<T, P>(string url, P? data = default)
    {
        await InitUser();
        var content = data != null ? serialize<P>(data) : null;
        var result = await client.PostAsync(url, content);
        return await Read<T>(result);
    }
    private async Task<string> Delete(string url)
    {
        await InitUser();
        var result = await client.DeleteAsync(url);
        return await result.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
    }

    /// <summary>
    /// Initialize or refresh user.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>
    public async Task<bool> InitUser()
    {
        string? token = Preferences.Get(Values.TOKEN_KEY, null);
        if (token != null)
        {
            var results = await TokenHandler.ValidateTokenAsync(token, ValidationParams);
            if (results.IsValid)
            {
                client.BearerToken = token;
                return true;
            }
        }

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = Values.JWT_ISS,
            Claims = new Dictionary<string, object>
            {
                { "identifier", AppIdentity.Id }
            },
            IssuedAt = null,
            NotBefore = null,
            Expires = DateTime.UtcNow.AddMinutes(2),
            SigningCredentials = TokenCredentials,
            Audience = Values.JWT_AUD
        };
        var initJwt = TokenHandler.CreateToken(descriptor);
        try
        {
            var req = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{Values.APP_SERVICE_URI}user/init"),
                Headers = { { "Authorization", $"Bearer {initJwt}" } }
            };
            var result = await client.SendAsync(req);
            var accessToken = await result.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
            if (accessToken != null)
            {
                Preferences.Set(Values.TOKEN_KEY, accessToken);
                client.BearerToken = accessToken;
                return true;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "InitUser failed.");
        }
        return false;
    }

    //
    // User
    //

    public async Task<Api.PbUser?> GetUser()
    {
        return await Get<Api.PbUser>("user");
    }
    public async Task<Api.PbUser?> UpdateUser(Api.PbUpdateUser update)
    {
        return await Put<Api.PbUser, Api.PbUpdateUser>("user", update);
    }
    public async void DeleteUser()
    {
        await Delete("user");
    }
    public async Task<Api.PbData?> GetData()
    {
        return await Get<Api.PbData>("user/data");
    }

    //
    // Vehicle
    //

    public async Task<List<Api.PbVehicle>> GetVehicles()
    {
        return await Get<List<Api.PbVehicle>>("vehicle") ?? [];
    }
    public async Task<Api.PbVehicle?> AddVehicle(Api.PbAddVehicle add)
    {
        return await Post<Api.PbVehicle, Api.PbAddVehicle>("vehicle", add);
    }
    public async Task<Api.PbVehicle?> UpdateVehicle(long id, Api.PbUpdateVehicle update)
    {
        return await Put<Api.PbVehicle, Api.PbUpdateVehicle>($"vehicle/{id}", update);
    }
    public async Task<string> DeleteVehicle(long id)
    {
        return await Delete($"vehicle/{id}");
    }

    //
    // Get geo cell and surrounding cell ids.
    //
    public async Task<List<string>?> GetGeoCellIds(double lat, double lon)
    {
        return await Get<List<string>>($"carpark/geo?lat={lat}&lon={lon}");
    }
    public async Task<Dictionary<string, Api.PbCarParks>?> GetGeoCells(List<string> cells)
    {
        return await Post<Dictionary<string, Api.PbCarParks>, List<string>>("carpark/geo", cells);
    }

    //
    // Parking
    //
    public async Task<Api.PbCarParks?> GetCarParks()
    {
        return await Get<Api.PbCarParks>("carpark");
    }
    public async Task<Api.PbTollParking?> AddTollParking(string id)
    {
        return await Post<Api.PbTollParking, object>($"carpark/toll?id={id}");
    }
    public async Task<string> DeleteTollParking(long id)
    {
        return await Delete($"carpark/toll/{id}");
    }
    public async Task<Api.PbKioskParking?> AddKioskParking(string id)
    {
        return await Post<Api.PbKioskParking, object>($"carpark/kiosk?id={id}");
    }
    public async Task<string> DeleteKioskParking(long id)
    {
        return await Delete($"carpark/kiosk/{id}");
    }

    //
    // Logs
    //
    public async Task<List<Api.ParkingOperationLog>> GetLogs()
    {
        return await Get<List<Api.ParkingOperationLog>>("logs") ?? [];
    }
    public async Task<Api.ParkingOperationLog?> AddLog(Api.ParkingLogCreate log)
    {
        return await Post<Api.ParkingOperationLog?, Api.ParkingLogCreate>("logs", log);
    }
}
