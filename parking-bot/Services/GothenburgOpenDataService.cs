using Microsoft.Extensions.Logging;

using ParkingBot.Models.Parking;
using ParkingBot.Properties;

using System.Net.Http.Json;

namespace ParkingBot.Services;

public class GothenburgOpenDataService(ILogger<GothenburgOpenDataService> logger, Http.HttpClientExt httpClient)
{
    private readonly string[] TollEndpoints = [Values.GBG_BASE_PUBL_TOLL_URI, Values.GBG_BASE_PRIV_TOLL_URI];

    /// <summary>
    /// Get nearest toll site info.
    /// </summary>
    /// <param name="maxDist">Maximum distance to concider (m)</param>
    /// <returns></returns>
    public async Task<List<TollSiteInfo>> GetNearestSiteInfosAsync(double lat, double lon, double maxDist)
    {
        List<TollSiteInfo> sites = [];
        foreach (var endpoint in TollEndpoints)
        {
            var url = RenderUrl(endpoint, lat: lat, lon: lon, radius: maxDist);
            var results = await httpClient.GetFromJsonAsync<List<TollSiteInfo>>(url);
            if (results is List<TollSiteInfo> tollSites)
            {
                foreach (var site in tollSites)
                {
                    var dist = Location.CalculateDistance(lat, lon, site.Lat, site.Long, DistanceUnits.Kilometers) * 1000;
                    if (dist < maxDist)
                    {
                        sites.Add(site);
                    }
                }
            }
        }
        return sites;
    }
    /// <summary>
    /// Get toll site info by id.
    /// </summary>
    /// <param name="id">Site id</param>
    /// <returns></returns>
    public async Task<ISiteInfo?> GetSiteInfoAsync(string id)
    {
        string[] endpoints = { Values.GBG_BASE_PRIV_TOLL_URI, Values.GBG_BASE_PUBL_TOLL_URI };
        logger.LogInformation("GothenburgOpenDataService.GetSiteInfoAsync()");
        foreach (var endpoint in TollEndpoints)
        {
            var site = await httpClient.GetFromJsonAsync<TollSiteInfo>(RenderUrl($"{endpoint}/{id}"));
            if (site != null)
            {
                return site;
            }
        }
        return null;
    }

    /// <summary>
    /// Render url template.
    /// </summary>
    /// <param name="template"></param>
    /// <param name="baseUrl"></param>
    /// <param name="ext"></param>
    /// <returns></returns>
    private static string RenderUrl(string template, string? baseUrl = null, double? lat = null, double? lon = null, string? ext = null, double radius = 500)
    {
        var rendered = template
            .Replace("{APPID}", Values.GBG_APP_ID)
            .Replace("{RADIUS}", radius.ToString());
        if (baseUrl != null)
        {
            rendered = rendered.Replace("{BASE}", baseUrl);
        }
        if (lat is double && lon is double)
        {
            rendered = rendered
                .Replace("{LATITUDE}", lat.ToString())
                .Replace("{LONGITUDE}", lon.ToString());
        }
        if (ext != null)
        {
            rendered = rendered.Replace("{EXT}", ext);
        }
        return rendered;
    }
}
