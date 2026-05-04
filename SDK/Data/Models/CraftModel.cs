using System.Collections.Generic;

namespace GraveSDK.Data.Models
{
    /// <summary>
    /// Type-safe model for crafting recipes
    /// </summary>
    public class CraftModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public List<CraftIngredient> Needs { get; set; }
        public List<CraftResult> Output { get; set; }
        public float Difficulty { get; set; }
        public List<string> RequiredPerks { get; set; }
        public List<string> LinkedBuffs { get; set; }
        public bool IsLocked { get; set; }
        public bool NeedsUnlock { get; set; }
        public bool CanAutoCraft { get; set; }
        public bool IsAuto { get; set; }
        public float EnergyCost { get; set; }
        public float TimeCost { get; set; }
        public float GratitudePointsCost { get; set; }
        public string TabId { get; set; }
        public CraftType CraftType { get; set; }
        public CraftSubType SubType { get; set; }
        public bool CanCraftMultiple { get; set; }
    }
}
