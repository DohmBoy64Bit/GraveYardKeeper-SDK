namespace GraveSDK.Data.Models
{
    public enum DayOfWeek
    {
        Pride = 0,
        Lust = 1,
        Gluttony = 2,
        Envy = 3,
        Wrath = 4,
        Sloth = 5
    }

    public class EnvironmentState
    {
        public int DayCount { get; set; }
        public DayOfWeek CurrentDay { get; set; }
        public float TimeOfDayK { get; set; } // 0.0 to 1.0
        public bool IsNight { get; set; }
        public bool IsRainy { get; set; }
    }
}
