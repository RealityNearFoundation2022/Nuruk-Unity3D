using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Proyecto26;
using RSG;


[System.Serializable]
public class currentWeatherLocation
{
    public currentWeather location;
}
[System.Serializable]
public class currentWeather
{
    public string name;
    public string region;
    public string country;
    public float lat;
    public float lon;
    public string tz_id;
    public int localtime_epoch;
    public string localtime; 
}
public class WeatherApi : MonoBehaviour
{
    private RequestHelper currentRequest;
    public static currentWeatherLocation weatherLocation = new currentWeatherLocation();
    private readonly string baseUri = "http://api.weatherapi.com/v1/";
    private readonly string APIKey = "c8302dbbd34049c898b222539220106";

    public RSG.IPromise<Proyecto26.ResponseHelper> GetWeather()
    {
        RestClient.DefaultRequestHeaders["Content-Type"] = "application/json";
        currentRequest = new RequestHelper
        {
            Uri = baseUri + $"timezone.json?key={APIKey}&q=Peru",
        };
        return RestClient.Get(currentRequest);
    }
}
