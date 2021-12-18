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
        }

        private void OnDisable()
        {
            TimeManager.OnNewDay -= DateTimeChanged;
        }

        private void Start()
        {
            DescriptionText = setDescriptionText;
            DescriptionText.text = "";
            previousDateTime = TimeManager.DateTime;
            SortDates();
            FillPanels((Season)0);
            SetKeyDateWeather();
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

        private void SetKeyDateWeather()
        {
            foreach (var date in keyDates)
            {
                if (date.weather != null && date.KeyDate.Weather == Weather.None)
                {
                    Debug.Log("Need set weather of" + date.KeyDate.Season + " " + date.KeyDate.Date);
                    date.KeyDate.Weather = date.weather;
                }
            }
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
