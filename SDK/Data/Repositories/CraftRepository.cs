using System;
using System.Collections.Generic;
using System.Linq;
using GraveSDK.Data.Models;

namespace GraveSDK.Data.Repositories
{
    /// <summary>
    /// Repository for accessing craft data
    /// Provides type-safe wrappers around CraftDefinition
    /// </summary>
    public class CraftRepository
    {
        private readonly Dictionary<string, CraftModel> _cache;

        public CraftRepository()
        {
            _cache = new Dictionary<string, CraftModel>();
        }

        /// <summary>
        /// Get a single craft by ID
        /// </summary>
        public CraftModel GetCraft(string craftId)
        {
            if (string.IsNullOrEmpty(craftId))
                return null;

            // Check cache first
            if (_cache.TryGetValue(craftId, out var cached))
                return cached;

            // Get from game data
            CraftDefinition def = GameBalance.me?.GetData<CraftDefinition>(craftId);
            if (def == null)
                return null;

            var model = ConvertToModel(def);
            _cache[craftId] = model;
            return model;
        }

        /// <summary>
        /// Get all crafts in the game
        /// </summary>
        public List<CraftModel> GetAllCrafts()
        {
            if (GameBalance.me?.craft_data == null)
                return new List<CraftModel>();

            return GameBalance.me.craft_data
                .Select(d => GetCraft(d.id))
                .Where(m => m != null)
                .ToList();
        }

        /// <summary>
        /// Get crafts by output item ID
        /// </summary>
        public List<CraftModel> GetCraftsByOutput(string outputItemId)
        {
            return GetAllCrafts()
                .Where(c => c.Output.Any(o => o.ItemId == outputItemId))
                .ToList();
        }

        /// <summary>
        /// Get crafts by craft type
        /// </summary>
        public List<CraftModel> GetCraftsByType(CraftType type)
        {
            return GetAllCrafts()
                .Where(c => c.CraftType == type)
                .ToList();
        }

        /// <summary>
        /// Get crafts that produce a specific item
        /// </summary>
        public List<CraftModel> GetCraftsForItem(string itemId)
        {
            return GetAllCrafts()
                .Where(c => c.Needs.Any(n => n.ItemId == itemId))
                .ToList();
        }

        /// <summary>
        /// Get unlocked crafts
        /// </summary>
        public List<CraftModel> GetUnlockedCrafts()
        {
            return GetAllCrafts()
                .Where(c => !c.IsLocked)
                .ToList();
        }

        /// <summary>
        /// Get crafts from a specific tab
        /// </summary>
        public List<CraftModel> GetCraftsFromTab(string tabId)
        {
            return GetAllCrafts()
                .Where(c => c.TabId == tabId)
                .ToList();
        }

        /// <summary>
        /// Get all unique craft IDs
        /// </summary>
        public List<string> GetAllIds()
        {
            return GetAllCrafts().Select(c => c.Id).ToList();
        }

        /// <summary>
        /// Convert CraftDefinition to CraftModel
        /// </summary>
        private CraftModel ConvertToModel(CraftDefinition def)
        {
            return new CraftModel
            {
                Id = def.id,
                DisplayName = def.GetNameNonLocalized(),
                Needs = ConvertNeeds(def.needs),
                Output = ConvertOutput(def.output),
                Difficulty = def.difficulty,
                RequiredPerks = new List<string>(def.linked_perks ?? new List<string>()),
                LinkedBuffs = new List<string>(def.linked_buffs ?? new List<string>()),
                IsLocked = def.IsLocked(),
                NeedsUnlock = def.needs_unlock,
                CanAutoCraft = def.is_auto,
                IsAuto = def.is_auto,
                EnergyCost = GetEnergyCost(def),
                TimeCost = GetTimeCost(def),
                GratitudePointsCost = GetGratitudePointsCost(def),
                TabId = def.tab_id,
                CraftType = (GraveSDK.Data.Models.CraftType)def.craft_type,
                SubType = (GraveSDK.Data.Models.CraftSubType)def.sub_type,
                CanCraftMultiple = def.CanCraftMultiple()
            };
        }

        /// <summary>
        /// Convert needs list
        /// </summary>
        private List<CraftIngredient> ConvertNeeds(List<Item> needs)
        {
            return needs?.Select(n => new CraftIngredient
            {
                ItemId = n.id,
                Count = (int)n.value,
                IsMultiquality = n.is_multiquality
            }).ToList() ?? new List<CraftIngredient>();
        }

        /// <summary>
        /// Convert output list
        /// </summary>
        private List<CraftResult> ConvertOutput(List<Item> output)
        {
            return output?.Select(o => new CraftResult
            {
                ItemId = o.id,
                Count = (int)o.value,
                IsMultiquality = o.is_multiquality
            }).ToList() ?? new List<CraftResult>();
        }

        /// <summary>
        /// Calculate energy cost
        /// </summary>
        private float GetEnergyCost(CraftDefinition def)
        {
            if (def.energy == null || !def.energy.has_expression)
                return 0f;

            return def.energy.EvaluateFloat(null, null);
        }

        /// <summary>
        /// Calculate time cost
        /// </summary>
        private float GetTimeCost(CraftDefinition def)
        {
            if (def.craft_time == null || !def.craft_time.has_expression)
                return 0f;

            return def.craft_time.EvaluateFloat(null, null);
        }

        /// <summary>
        /// Calculate gratitude points cost
        /// </summary>
        private float GetGratitudePointsCost(CraftDefinition def)
        {
            if (def.gratitude_points_craft_cost == null || !def.gratitude_points_craft_cost.has_expression)
                return 0f;

            return def.gratitude_points_craft_cost.EvaluateFloat(null, null);
        }

        /// <summary>
        /// Clear the cache
        /// </summary>
        public void ClearCache()
        {
            _cache.Clear();
        }
    }
}
