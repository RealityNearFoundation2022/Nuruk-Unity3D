
using UnityEngine;
using System;
using Newtonsoft.Json;


public class CycleSkyBoxes : MonoBehaviour
{
    WeatherApi api;
    string localtime;

    void Start()
    {
      api = gameObject.GetComponent<WeatherApi>();
        Request();
    }

    public void Request(){
        api.GetWeather().Then((res) => {
           WeatherApi.weather = res;
            string jsonString = JsonConvert.SerializeObject(res);
            Debug.Log(jsonString);
        }).Catch(er => {
            Debug.Log(er.Message);
        });
        
    }

}
