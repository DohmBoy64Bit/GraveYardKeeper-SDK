using System.Collections.Generic;

namespace GraveSDK.Data.Models
{
    /// <summary>
    /// Type-safe model for buffs/status effects
    /// </summary>
    public class BuffModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public float Duration { get; set; }
        public bool IsHidden { get; set; }
        public Dictionary<string, float> ResourceEffects { get; set; }
        public string CustomIcon { get; set; }
        public float TickPeriod { get; set; }
        public bool DoNotShowTimer { get; set; }
        public float CraftQualityBonus { get; set; }
        public BuffOverlayType OverlayType { get; set; }
    }
}
