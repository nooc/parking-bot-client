using Microsoft.Extensions.Logging;

using ParkingBot.Models.Parking;
using ParkingBot.Properties;

using System.Net.Http.Json;

namespace ParkingBot.Services;

public class GothenburgOpenDataService(ILogger<GothenburgOpenDataService> logger, Http.HttpClientExt httpClient)
{
    private readonly string[] TollEndpoints = [Values.GBG_BASE_PRIV_TOLL_URI, Values.GBG_BASE_PUBL_TOLL_URI];

    /// <summary>
    /// Get nearest toll site info.
    /// </summary>
    /// <param name="loc">Query location</param>
    /// <param name="maxDist">Maximum distance to concider (m)</param>
    /// <returns></returns>
    public async Task<ISiteInfo?> GetNearestSiteInfoAsync(Location loc, double maxDist)
    {
        TollSiteInfo? selectedSite = null;
        logger.LogInformation("GothenburgOpenDataService.GetNearestSiteInfoAsync()");
        double selectedDist = double.MaxValue;
        foreach (var endpoint in TollEndpoints)
        {
            var sites = await httpClient.GetFromJsonAsync<List<TollSiteInfo>>(RenderUrl(endpoint, loc: loc));
            if (sites != null)
            {

                foreach (var site in sites)
                {
                    if (site != null)
                    {
                        var dist = Location.CalculateDistance(loc.Latitude, loc.Longitude, site.Lat, site.Long, DistanceUnits.Kilometers) * 1000;
                        if (dist < selectedDist)
                        {
                            selectedDist = dist;
                            selectedSite = site;
                        }
                    }
                }
            }
        }
        return selectedDist <= maxDist ? selectedSite : null;
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
    /// <param name="loc"></param>
    /// <param name="ext"></param>
    /// <returns></returns>
    private static string RenderUrl(string template, string? baseUrl = null, Location? loc = null, string? ext = null)
    {
        var rendered = template
            .Replace("{APPID}", Values.GBG_APP_ID)
            .Replace("{RADIUS}", Values.GPS_REGION_RADIUS.ToString());
        if (baseUrl != null)
        {
            rendered = rendered.Replace("{BASE}", baseUrl);
        }
        if (loc != null)
        {
            rendered = rendered
                .Replace("{LATITUDE}", loc.Latitude.ToString())
                .Replace("{LONGITUDE}", loc.Longitude.ToString());
        }
        if (ext != null)
        {
            rendered = rendered.Replace("{EXT}", ext);
        }
        return rendered;
    }
}
