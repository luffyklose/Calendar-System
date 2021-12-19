using System.Collections.Generic;
using UnityEngine;
using DPUtils.Systems.DateTime;
using UnityEngine.UI;

public class WeatherManager : MonoBehaviour
{
    public static Weather currentWeather = Weather.Sunny;

    public List<ParticleSystem> PSList;
    public ParticleSystem rainParticles;
    public ParticleSystem snowParticles;

    private void Awake()
    {
        PSList = new List<ParticleSystem>();
        PSList.Add(rainParticles);
        PSList.Add(snowParticles);

        StopAllPS();
    }

    private void OnEnable()
    {
        TimeManager.OnNewDay += SetWeather;
    }

    private void OnDisable()
    {
        TimeManager.OnNewDay -= SetWeather;
    }

    private void SetWeather(DateTime dateTime)
    {
        //Debug.Log("Set weather" + dateTime.Weather);
        if (dateTime.Weather != Weather.None)
        {
            currentWeather = dateTime.Weather;
        }
        else
        {
            //Debug.Log("Random weather");
            currentWeather = (Weather)Random.Range(1, (int)Weather.MAX_WEATHER_AMOUNT + 1);
        }
        
        StopAllPS();
        switch (currentWeather)
        {
            case Weather.Raining:
            {
                rainParticles.Play();
                break;
            }
            case Weather.Snowing:
            {
                snowParticles.Play();
                break;
            }
            case Weather.Sunny:
            {
                break;
            }
            default: break;
        }
    }

    private void StopAllPS()
    {
        foreach (ParticleSystem ps in PSList)
        {
            ps.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}