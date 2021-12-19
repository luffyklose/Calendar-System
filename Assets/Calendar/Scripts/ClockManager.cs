using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;
using TMPro;
using DPUtils.Systems.DateTime;

public class ClockManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI timeText;
    public Image weatherSprite;
    public Sprite[] weatherSprites;
    
    [Header("SunLight")]
    public Light2D sunlight;
    public float nightIntensity;
    public float dayIntensity;
    public Color dayColor;
    public Color rainyDayColor;
    public Color nightColor;
    public AnimationCurve dayNightCurve;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        TimeManager.OnDateTimeChanged += UpdateDateTime;
        TimeManager.OnNewDay += UpdateWeather;
    }

    private void OnDisable()
    {
        TimeManager.OnDateTimeChanged -= UpdateDateTime;
        TimeManager.OnNewDay -= UpdateWeather;
    }
    
    private void UpdateDateTime(DateTime dateTime)
    {
        dateText.text = dateTime.DateToString();
        timeText.text = dateTime.TimeToString();

        float t = (float)dateTime.Hour / 24f;

        float dayNightT = dayNightCurve.Evaluate(t);

        sunlight.intensity = Mathf.Lerp(dayIntensity, nightIntensity, dayNightT);

        if(WeatherManager.currentWeather == Weather.Sunny)
            sunlight.color = Color.Lerp(dayColor, nightColor, dayNightT);
        else sunlight.color = Color.Lerp(rainyDayColor, nightColor, dayNightT);
    }

    private void UpdateWeather(DateTime dateTime)
    {
        //Debug.Log($"{WeatherManager.currentWeather} {TimeManager.DateTime.Weather}");
        weatherSprite.sprite = weatherSprites[(int)WeatherManager.currentWeather];
    }
}
