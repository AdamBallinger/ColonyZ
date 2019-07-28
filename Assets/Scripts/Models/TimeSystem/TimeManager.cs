using UnityEngine;

namespace Models.TimeSystem
{
    public static class TimeManager
    {
        /// <summary>
        /// Controls the time scale mode.
        /// </summary>
        public static TimeMode TimeMode { get; set; } = TimeMode.x1;

        /// <summary>
        /// Returns the delta time based on the current time mode.
        /// </summary>
        public static float DeltaTime => Time.deltaTime * (int)TimeMode;
    }
}