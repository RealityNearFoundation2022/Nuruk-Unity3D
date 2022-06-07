using System;
using UnityEngine;
using Newtonsoft.Json;


public class CycleSkyBoxes : MonoBehaviour
{
    public Material[] SkyBoxes;
    WeatherApi api;
    string localtime;
    string  time;
    int count = 0, prev, current;
    public int hour;

    void Start()
    {
        api = gameObject.GetComponent<WeatherApi>();
        Request();
    }

    public void Request(){
        api.GetWeather().Then((res) => {
           WeatherApi.weatherLocation =  JsonConvert.DeserializeObject<currentWeatherLocation>(res.Text);
           localtime = WeatherApi.weatherLocation.location.localtime.Split(' ')[1];
            time = localtime.Split(':')[0];
            hour =  int.Parse(time);
            prev = hour;
           setSkybox();
        }).Catch(er => {
            Debug.Log(er.Message);
        });
    }
    void setSky(Material sky){
        RenderSettings.skybox = sky;
    }
    void  setSkybox() {
        // de 0 a 3 am 
        if ( hour < 4 ){
            setSky(SkyBoxes[0]);
        }
        // de 4 a 5 am 
        else if ( hour == 4 || hour == 5){
            setSky(SkyBoxes[1]);
        }
        // de 6 a 7 am 
        else if ( hour == 6 || hour == 7){
            setSky(SkyBoxes[2]);
        }
        // de 8 a 9 am 
        else if ( hour == 8 || hour == 9){
            setSky(SkyBoxes[3]);
        }
        // de 10 a 11 am 
        else if (  hour == 10 || hour == 11){
            setSky(SkyBoxes[4]);
        }
        // de 12 a 13 pm
        else if ( hour == 12 || hour == 13){
            setSky(SkyBoxes[5]);
        }
        // de 14 a 15 pm 
        else if ( hour == 14 || hour == 15){
            setSky(SkyBoxes[6]);
        }
        // de 16 a 17 pm  
        else if (  hour == 16 || hour == 17){
            setSky(SkyBoxes[7]);
        }
        // de 18  pm 
        else if (  hour == 18){
            setSky(SkyBoxes[8]);
        }
        // de 19  pm 

        else if (hour == 19){
            setSky(SkyBoxes[9]);
        }
        // de 20 a 23 pm 
        else if (  hour > 19 && hour < 24){
            setSky(SkyBoxes[10]);

        }else
        {
            setSky(SkyBoxes[4]);
        }
    }
    private void Update() {
        current = hour;
        count = 0;
        if(prev != current && count < 1){
            setSkybox();
            count++;
        }
    }
}
