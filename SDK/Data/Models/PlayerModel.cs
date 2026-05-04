using UnityEngine;

namespace GraveSDK.Data.Models
{
    public class PlayerStats
    {
        public float Health { get; set; }
        public float HealthMax { get; set; }
        public float Energy { get; set; }
        public float EnergyMax { get; set; }
        public float Sanity { get; set; }
        public float SanityMax { get; set; }
        public float Money { get; set; }
        public float GratitudePoints { get; set; }
        
        public int TechRed { get; set; }
        public int TechGreen { get; set; }
        public int TechBlue { get; set; }
    }
}
