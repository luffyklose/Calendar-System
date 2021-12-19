using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

namespace DPUtils.Systems.DateTime
{
    public class CalendarController : MonoBehaviour
    {
        public List<DayBox> calendarPanels;
        public List<KeyDates> keyDates;
        public TextMeshProUGUI seasonText;
        public TextMeshProUGUI setDescriptionText;
        public TextMeshProUGUI yearText;
        public static TextMeshProUGUI DescriptionText;

        private int currentSeasonView = 0;
        private int currentYear = 1;
        private DateTime previousDateTime;

        private void Awake()
        {
            TimeManager.OnNewDay += DateTimeChanged;
            TimeManager.PrepareNewDay += SetWeather;
        }

        private void OnDisable()
        {
            TimeManager.OnNewDay -= DateTimeChanged;
            TimeManager.PrepareNewDay -= SetWeather;
        }

        private void Start()
        {
            DescriptionText = setDescriptionText;
            DescriptionText.text = "";
            previousDateTime = TimeManager.DateTime;
            SortDates();
            FillPanels((Season)currentSeasonView);
        }

        void DateTimeChanged(DateTime _date)
        {
            if (currentSeasonView == (int)_date.Season && currentYear==_date.Year)
            {
                if (previousDateTime.Date != _date.Date)
                {
                    var index = (previousDateTime.Date - 1) < 0 ? 0 : (previousDateTime.Date - 1);
                    calendarPanels[index].HideHighlight();
                    calendarPanels[_date.Date - 1].ShowHighlight();
                }
                calendarPanels[_date.Date - 1].ShowHighlight();
                previousDateTime = _date;
            }
            else if (currentYear == _date.Year)
            {
                currentSeasonView++;
                FillPanels(_date.Season);
                calendarPanels[0].ShowHighlight();
                previousDateTime = _date;
            }
            else
            {
                currentYear++;
                currentSeasonView = 0;
                FillPanels(_date.Season);
                calendarPanels[0].ShowHighlight();
                previousDateTime = _date;
            }
        }

        void SetWeather(DateTime _date)
        {
            //Debug.Log("Start setting weather");
            foreach (KeyDates date in this.keyDates)
            {
                bool isNextDayKey = (date.KeyDate.Date == _date.Date + 1 && date.KeyDate.Season == _date.Season &&
                                     (date.KeyDate.Year == _date.Year || date.Yearly)) ||
                                    (date.KeyDate.Date == 1 && _date.Date == 28 &&
                                     date.KeyDate.Season == _date.Season + 1 &&
                                     (date.KeyDate.Year == _date.Year || date.Yearly)) ||
                                    (date.KeyDate.Date == 1 && _date.Date == 28 && date.KeyDate.Season == (Season)0 &&
                                     _date.Season == (Season)3 &&
                                     (date.KeyDate.Year == _date.Year + 1 || date.Yearly));
                /*
                Debug.Log(
                    $"Date: {date.KeyDate.Date} {_date.Date} Season: {date.KeyDate.Season} {_date.Season} Year: {date.KeyDate.Year} {_date.Year}");
                Debug.Log((date.KeyDate.Date == _date.Date + 1 && date.KeyDate.Season == _date.Season &&
                           (date.KeyDate.Year == _date.Year || date.Yearly)) + " " + (date.KeyDate.Date == 1 &&
                              _date.Date == 28 &&
                              date.KeyDate.Season == _date.Season + 1 &&
                              (date.KeyDate.Year == _date.Year || date.Yearly)) + " " +
                          (date.KeyDate.Date == 1 && _date.Date == 28 && date.KeyDate.Season == (Season)0 &&
                           _date.Season == (Season)3 &&
                           (date.KeyDate.Year == _date.Year + 1 || date.Yearly)));
                Debug.Log($"{date.KeyDate.Date} {date.KeyDate.Season} is KeyDate? {isNextDayKey}!");
                */
                if (isNextDayKey && date.KeyDate.Weather!=Weather.None)
                {
                    TimeManager.DateTime.Weather = date.weather;
                    //Debug.Log($"Set {TimeManager.DateTime.Weather} succeed on {date.KeyDate.Date} {date.KeyDate.Season} {date.KeyDate.Year}");
                    return;
                }
            }
            TimeManager.DateTime.Weather = Weather.None;
        }

        private void SortDates()
        {
            keyDates = keyDates
                .OrderBy(d => d.KeyDate.Season)
                .ThenBy(d => d.KeyDate.Date)
                .ToList();
        }

        private void FillPanels(Season _season)
        {
            seasonText.text = _season.ToString();
            yearText.text = "YEAR " + currentYear.ToString();

            for (int i = 0; i < calendarPanels.Count; i++)
            {
                calendarPanels[i].SetUpDate((i + 1).ToString());

                if (currentSeasonView == (int)TimeManager.DateTime.Season && (i + 1) == TimeManager.DateTime.Date && currentYear==TimeManager.DateTime.Year)
                {
                    calendarPanels[i].ShowHighlight();
                }
                else
                {
                    calendarPanels[i].HideHighlight();
                }

                foreach (var date in keyDates)
                {
                    if ((i + 1) == date.KeyDate.Date && date.KeyDate.Season == _season &&
                        (date.Yearly || date.KeyDate.Year == currentYear))
                    {
                        calendarPanels[i].AssignKeyDate(date);
                    }
                }
            }
        }

        public void SetInitialTime(Season season,int year)
        {
            currentSeasonView = (int)season;
            currentYear = year;
        }
        
        public void OnNextSeason()
        {
            currentSeasonView += 1;
            if (currentSeasonView > 3)
            {
                currentSeasonView = 0;
                currentYear++;
            }
            FillPanels((Season)currentSeasonView);
        }

        public void OnPreviousSeason()
        {
            if (currentYear == 1 && currentSeasonView == 0)
            {
                return;
            }
            currentSeasonView -= 1;
            if (currentSeasonView < 0)
            {
                currentSeasonView = 3;
                currentYear--;
            }
            FillPanels((Season)currentSeasonView);
        }

    }

}
