using System;
using UnityEngine;

namespace ColonyZ.Models.TimeSystem
{
    public class TimeManager
    {
        public static TimeManager Instance { get; private set; }

        /// <summary>
        ///     Controls the time scale mode.
        /// </summary>
        public TimeMode TimeMode
        {
            get => timeMode;
            set
            {
                if (value != timeMode)
                {
                    timeMode = value;
                    timeModeChangedEvent?.Invoke(value);
                }
            }
        }

        /// <summary>
        ///     Returns the delta time based on the current time mode.
        /// </summary>
        public float DeltaTime => Time.deltaTime * (int) TimeMode;

        /// <summary>
        ///     Returns normal delta time regardless of the current time mode.
        /// </summary>
        public float UnscaledDeltaTime => Time.deltaTime;

        /// <summary>
        ///     Returns the number of times the clock has ticked for the current day. Resets at 00:00
        /// </summary>
        public float Ticks => Hour * 60 + Minute;

        /// <summary>
        ///     Progress percentage ranging from 0-1 for the current day (00:00 -> 23:59)
        /// </summary>
        public float DayProgress => Ticks / (24 * 60);

        public int Hour { get; private set; }

        public int Minute { get; private set; }

        public int Day { get; private set; }

        /// <summary>
        ///     Event called when the current time mode changes. Passes the new mode as a parameter.
        /// </summary>
        public event Action<TimeMode> timeModeChangedEvent;

        /// <summary>
        ///     Event called when the time changed. Passes the current hour/minute as parameters.
        /// </summary>
        public event Action<int, int> timeChangedEvent;

        /// <summary>
        ///     Event called when a new day (Time reaches 00:00) starts. The current day number is passed as a parameter.
        /// </summary>
        public event Action<int> newDayEvent;

        /// <summary>
        ///     How many game time seconds pass per real life second.
        /// </summary>
        private const int millisPerSecond = 1000;

        /// <summary>
        ///     Number of in game seconds per minute.
        /// </summary>
        private const int millisPerMinute = 1 * 1000;

        /// <summary>
        ///     Current milliseconds passed.
        /// </summary>
        public int millis;

        private TimeMode timeMode;

        private TimeMode toggleTimeMode;

        private TimeManager()
        {
        }

        /// <summary>
        ///     Create the TimeManager at the given time (24h).
        /// </summary>
        /// <param name="_hour"></param>
        /// <param name="_minute"></param>
        /// <param name="_day"></param>
        /// <param name="_timeMode"></param>
        public static void Create(int _hour, int _minute, int _day, TimeMode _timeMode = TimeMode.x1)
        {
            if (Instance == null)
                Instance = new TimeManager
                {
                    Hour = _hour, Minute = _minute, Day = _day, TimeMode = _timeMode
                };
        }

        public static void Destroy()
        {
            Instance = null;
        }

        public void SetTime(int _hour, int _minute, int _day)
        {
            _hour = Math.Min(_hour, 24);
            _minute = Math.Min(_minute, 59);

            if (Day != _day)
            {
                Day = _day;
                newDayEvent?.Invoke(Day);
            }

            Hour = _hour;
            Minute = _minute;
            millis = 0;

            timeChangedEvent?.Invoke(Hour, Minute);
        }

        /// <summary>
        ///     Toggles the TimeMode between 0 and the previously set time mode.
        /// </summary>
        public void Toggle()
        {
            if (TimeMode != TimeMode.x0)
            {
                toggleTimeMode = TimeMode;
                TimeMode = TimeMode.x0;
            }
            else
            {
                TimeMode = toggleTimeMode;
            }
        }

        public void Update()
        {
            millis += (int) (millisPerSecond * DeltaTime);

            // Check if a minute has passed.
            if (millis >= millisPerMinute)
            {
                Minute += 1 * (millis / millisPerMinute);
                millis -= millisPerMinute * (millis / millisPerMinute);

                // Check if an hour has passed.
                if (Minute >= 60)
                {
                    Hour += 1 * (Minute / 60);
                    Minute = 0;

                    // Check if a day has passed.
                    if (Hour >= 24)
                    {
                        Hour = 0;
                        Day++;
                        newDayEvent?.Invoke(Day);
                    }
                }

                timeChangedEvent?.Invoke(Hour, Minute);
            }
        }
    }
}