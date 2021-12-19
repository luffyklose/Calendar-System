using UnityEngine;
using UnityEngine.Events;

namespace DPUtils.Systems.DateTime
{
    public class TimeManager : MonoBehaviour
    {
        [Header("Date & Time Settings")]
        [Range(1, 28)]
        public int dateInMonth;
        [Range(1, 4)]
        public int season;
        [Range(1, 99)]
        public int year;
        [Range(0, 24)]
        public int hour;
        [Range(0, 6)]
        public int minutes;

        public static DateTime DateTime;

        [Header("Tick Settings")]
        public int TickMinutesIncreased = 10;
        public float TimeBetweenTicks = 1;
        private float currentTimeBetweenTicks = 0;

        public static UnityAction<DateTime> OnDateTimeChanged;
        public static UnityAction<DateTime> OnNewDay;
        public static UnityAction<DateTime> PrepareNewDay;

        private void Awake()
        {
            DateTime = new DateTime(dateInMonth, season - 1, year, hour, minutes * 10);
        }

        private void Start()
        {
            OnDateTimeChanged?.Invoke(DateTime);
            OnNewDay?.Invoke(DateTime);
            CalendarController calendarController = FindObjectOfType<CalendarController>();
            calendarController.SetInitialTime(DateTime.Season, DateTime.Year);
        }

        private void Update()
        {
            currentTimeBetweenTicks += Time.deltaTime;

            if (currentTimeBetweenTicks >= TimeBetweenTicks)
            {
                currentTimeBetweenTicks = 0;
                DateTime.AdvanceMinutes(TickMinutesIncreased);

                OnDateTimeChanged?.Invoke(DateTime);
            }
        }

        public void SkipToTomorrow()
        {
            DateTime.Hour = 0;
            DateTime.Minutes = 0;
            //Debug.Log($"Tmr {hour} {minutes}");
            OnDateTimeChanged?.Invoke(DateTime);
            DateTime.AdvanceDay();
        }
    }

    [System.Serializable]
    public struct DateTime
    {
        #region Fields
        private Days day;
        [SerializeField] private int date;
        [SerializeField] private int year;

        [SerializeField] private int hour;
        [SerializeField] private int minutes;

        [SerializeField] private Season season;

        private Weather weather;

        private int totalNumDays;
        private int totalNumWeeks;
        #endregion

        #region Properties
        public Days Day => day;
        public int Date => date;

        public int Hour
        {
            get { return hour; }
            set { hour = value; }
        }
        public int Minutes
        {
            get { return minutes; }
            set { minutes = value; }
        }
        public Season Season => season;
        public int Year => year;
        public int TotalNumDays => totalNumDays;
        public int TotalNumWeeks => totalNumWeeks;
        public int CurrentWeek => totalNumWeeks % 16 == 0 ? 16 : totalNumWeeks % 16;
        public Weather Weather
        {
            get { return weather; }
            set
            {
                weather = value;
                //Debug.Log($"Weather change to {value}");
            }
        }
        #endregion

        #region Constructors

        public DateTime(int date, int season, int year, int hour, int minutes)
        {
            this.day = (Days)(date % 7);
            if (day == 0) day = (Days)7;
            this.date = date;
            this.season = (Season)season;
            this.year = year;

            this.hour = hour;
            this.minutes = minutes;

            weather = Weather.None;

            totalNumDays = date + (28 * (int)this.season) + (28 * 4 * (year - 1));

            totalNumWeeks = 1 + totalNumDays / 7;
        }

        #endregion

        #region Time Advancement

        public void AdvanceMinutes(int SecondsToAdvanceBy)
        {
            if (minutes + SecondsToAdvanceBy >= 60)
            {
                minutes = (minutes + SecondsToAdvanceBy) % 60;
                AdvanceHour();
            }
            else
            {
                minutes += SecondsToAdvanceBy;
            }
        }

        private void AdvanceHour()
        {
            if ((hour + 1) == 24)
            {
                hour = 0;
                AdvanceDay();
            }
            else
            {
                hour++;
            }
        }

        public void AdvanceDay()
        {
            TimeManager.PrepareNewDay?.Invoke(TimeManager.DateTime);
            day++;

            if (day > (Days)7)
            {
                day = (Days)1;
                totalNumWeeks++;
            }

            date++;

            if (date % 29 == 0)
            {
                AdvanceSeason();
                date = 1;
            }

            totalNumDays++;
            //Debug.Log($"New Weather {TimeManager.DateTime.Weather}");
            TimeManager.OnNewDay?.Invoke(TimeManager.DateTime);
        }

        private void AdvanceSeason()
        {
            if (Season == Season.Winter)
            {
                season = Season.Spring;
                AdvanceYear();
            }
            else season++;
        }

        private void AdvanceYear()
        {
            date = 1;
            year++;
        }

        #endregion

        #region Bool Checks
        public bool IsWeekend()
        {
            return day > Days.Fri ? true : false;
        }

        public bool IsParticularDay(Days _day)
        {
            return day == _day;
        }
        #endregion

        #region Key Dates

        public DateTime NewYearsDay(int year)
        {
            if (year == 0) year = 1;
            return new DateTime(1, 0, year, 6, 0);
        }

        public DateTime GetParticualrDay(int year, Season season, int day)
        {
            if (year == 0) year = 1;
            return new DateTime(day, (int)season, year, 6, 0);
        }

        #endregion

        #region To Strings

        public override string ToString()
        {
            return $"Date: {DateToString()} Season: {season} Time: {TimeToString()} ";
        }
        public string DateToString()
        {
            var Day = day;
                return $"{Day} {Date}";
        }

        public string TimeToString()
        {
            var Hour = hour;
            return $"{Hour.ToString("D2")}:{minutes.ToString("D2")}";
        }

        #endregion
    }
}