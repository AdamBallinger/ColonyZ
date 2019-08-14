using UnityEngine;

namespace Models.TimeSystem
{
    public class TimeManager
    {
        public static TimeManager Instance { get; private set; }
        
        /// <summary>
        /// Controls the time scale mode.
        /// </summary>
        public TimeMode TimeMode { get; set; } = TimeMode.x1;

        /// <summary>
        /// Returns the delta time based on the current time mode.
        /// </summary>
        public float DeltaTime => Time.deltaTime * (int)TimeMode;
        
        public int Hour { get; private set; }
        
        public int Minute { get; private set; }
        
        public int Day { get; private set; }

        /// <summary>
        /// Current milliseconds passed.
        /// </summary>
        public int millis;

        /// <summary>
        /// How many game time seconds pass per real life second.
        /// </summary>
        private const int millisPerSecond = 1000;

        /// <summary>
        /// Number of in game seconds per minute.
        /// </summary>
        private const int millisPerMinute = 1 * 1000;
        
        private TimeManager() {}
        
        /// <summary>
        /// Create the TimeManager at the given time (24h).
        /// </summary>
        /// <param name="_hour"></param>
        /// <param name="_minute"></param>
        /// <param name="_day"></param>
        public static void Create(int _hour, int _minute, int _day)
        {
            if (Instance == null)
            {
                Instance = new TimeManager
                {
                    Hour = _hour, millis = _minute, Day = _day
                };
            }
        }
        
        public void Update()
        {
            millis += (int)(millisPerSecond * DeltaTime);
            
            // Check if a minute has passed.
            if (millis >= millisPerMinute)
            {
                Minute += 1 * (millis / millisPerMinute);
                millis -= millisPerMinute * (millis / millisPerMinute);
            }
            
            // Check if an hour has passed.
            if (Minute >= 60)
            {
                Hour += 1 * (Minute / 60);
                Minute = 0;
            }
            
            // Check if a day has passed.
            if (Hour >= 24)
            {
                Hour = 0;
                Day++;
            }
        }
    }
}