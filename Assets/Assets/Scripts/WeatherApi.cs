using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Proyecto26;
using RSG;


/*[System.Serializable]
 public class currentWeatherLocation
{
    public Dictionary<string, dynamic> location;
} */
[System.Serializable]
public class currentWeather
{
    public string name { get; set; }
    public string region { get; set; }
    public string country { get; set; }
    public int lat { get; set; }
    public int lon { get; set; }
    public string tz_id { get; set; }
    public int localtime_epoch { get; set; }
    public string localtime { get; set; } 
}
public class WeatherApi : MonoBehaviour
{
    private RequestHelper currentRequest;
    public static currentWeather weather = new currentWeather();
   // public static currentWeatherLocation weatherLocation = new currentWeatherLocation();
    private readonly string baseUri = "http://api.weatherapi.com/v1/";
    private readonly string APIKey = "c8302dbbd34049c898b222539220106";

    public RSG.IPromise<currentWeather> GetWeather()
    {
        RestClient.DefaultRequestHeaders["Content-Type"] = "application/json";
        currentRequest = new RequestHelper
        {
            Uri = baseUri + $"timezone.json?key={APIKey}&q=Peru",
        };
        return RestClient.Get<currentWeather>(currentRequest);
    }
}
