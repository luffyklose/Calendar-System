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

        rainParticles.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        snowParticles.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        
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
        Debug.Log("Set weather");
       if (dateTime.Hour == 0 && dateTime.Minutes == 0)
        {
            if (dateTime.Weather != Weather.None)
            {
                currentWeather = dateTime.Weather;
            }
            else
            {
                Debug.Log("Random weather");
                currentWeather = (Weather)Random.Range(1, (int)Weather.MAX_WEATHER_AMOUNT + 1);
            }

            switch (currentWeather)
            {
                case Weather.Raining:
                {
                    rainParticles.Play();
                    break;
                }
                case Weather.Sonwing:
                {
                    snowParticles.Play();
                    break;
                }
                case Weather.Sunny:
                {
                    StopAllPS();
                    break;
                }
                default: break;
            }
        }
    }

    private void StopAllPS()
    {
        foreach (var ps in PSList)
        {
            ps.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}