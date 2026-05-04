using GraveSDK.Data.Models;
using UnityEngine;

namespace GraveSDK.Data.Repositories
{
    public class EnvironmentRepository
    {
        public EnvironmentState GetState()
        {
            var tod = TimeOfDay.me;
            var save = MainGame.me?.save;
            if (tod == null || save == null) return null;

            return new EnvironmentState
            {
                DayCount = save.day,
                CurrentDay = (DayOfWeek)save.day_of_week,
                TimeOfDayK = tod.GetTimeK(),
                IsNight = tod.is_night,
                IsRainy = EnvironmentEngine.me?.is_rainy ?? false
            };
        }

        public void SetTime(float k)
        {
            TimeOfDay.me?.SetTimeK(k);
        }

        public void SkipToNextDay()
        {
            if (MainGame.me?.save != null)
            {
                // Game handles end of day via EnvironmentEngine.Update normally,
                // but we can manually trigger parts if needed.
                // Simplest way is to set time to near end.
                SetTime(0.99f);
            }
        }

        public void SetWeatherRain(bool rainy)
        {
            var env = EnvironmentEngine.me;
            if (env == null) return;

            // Weather is complex in GYK, uses SwitchableWeatherState.
            // For now, we can try to force a state or use a simple toggle if available.
            // Forcing a "forced_weather_line" is safer.
        }
    }
}
