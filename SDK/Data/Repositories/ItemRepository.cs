using System;
using System.Collections.Generic;
using System.Linq;
using GraveSDK.Data.Models;

namespace GraveSDK.Data.Repositories
{
    /// <summary>
    /// Repository for accessing item data
    /// Provides type-safe wrappers around ItemDefinition
    /// </summary>
    public class ItemRepository
    {
        private readonly Dictionary<string, ItemModel> _cache;

        public ItemRepository()
        {
            _cache = new Dictionary<string, ItemModel>();
        }

        /// <summary>
        /// Get a single item by ID
        /// </summary>
        public ItemModel GetItem(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
                return null;

            // Check cache first
            if (_cache.TryGetValue(itemId, out var cached))
                return cached;

            // Get from game data
            ItemDefinition def = GameBalance.me?.GetData<ItemDefinition>(itemId);
            if (def == null)
                return null;

            var model = ConvertToModel(def);
            _cache[itemId] = model;
            return model;
        }

        /// <summary>
        /// Get all items in the game
        /// </summary>
        public List<ItemModel> GetAllItems()
        {
            if (GameBalance.me?.items_data == null)
                return new List<ItemModel>();

            return GameBalance.me.items_data
                .Select(d => GetItem(d.id))
                .Where(m => m != null)
                .ToList();
        }

        /// <summary>
        /// Get items by type
        /// </summary>
        public List<ItemModel> GetItemsByType(ItemType type)
        {
            return GetAllItems()
                .Where(i => i.Type == type)
                .ToList();
        }

        /// <summary>
        /// Get items by name (case-insensitive partial match)
        /// </summary>
        public List<ItemModel> SearchItems(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return GetAllItems();

            string lower = searchTerm.ToLower();
            return GetAllItems()
                .Where(i => i.DisplayName.ToLower().Contains(lower) ||
                           i.Id.ToLower().Contains(lower))
                .ToList();
        }

        /// <summary>
        /// Get items that can be used as tools
        /// </summary>
        public List<ItemModel> GetTools()
        {
            return GetAllItems()
                .Where(i => i.IsTool)
                .ToList();
        }

        /// <summary>
        /// Get items that are weapons
        /// </summary>
        public List<ItemModel> GetWeapons()
        {
            return GetAllItems()
                .Where(i => i.IsWeapon)
                .ToList();
        }

        /// <summary>
        /// Get items that can be equipped
        /// </summary>
        public List<ItemModel> GetEquipment()
        {
            return GetAllItems()
                .Where(i => i.IsEquipment)
                .ToList();
        }

        /// <summary>
        /// Get all unique item IDs
        /// </summary>
        public List<string> GetAllIds()
        {
            return GetAllItems().Select(i => i.Id).ToList();
        }

        /// <summary>
        /// Convert ItemDefinition to ItemModel
        /// </summary>
        private ItemModel ConvertToModel(ItemDefinition def)
        {
            return new ItemModel
            {
                Id = def.id,
                DisplayName = def.GetItemName(true),
                Description = def.GetItemDescription(null),
                Type = (GraveSDK.Data.Models.ItemType)def.type,
                Quality = def.quality,
                StackCount = def.stack_count,
                BasePrice = def.base_price,
                HasDurability = def.has_durability,
                DurabilityDecrease = def.durability_decrease,
                Parameters = ConvertGameRes(def.parameters),
                OnUseEffects = def.on_use_expressions?.Select(e => e.GetRawExpressionString())
                    ?.ToList() ?? new List<string>(),
                CanBeUsed = def.can_be_used,
                IsTool = def.is_tool,
                IsWeapon = def.IsWeapon(),
                IsEquipment = def.IsEquipment()
            };
        }

        /// <summary>
        /// Convert GameRes to dictionary
        /// </summary>
        private Dictionary<string, float> ConvertGameRes(GameRes res)
        {
            var dict = new Dictionary<string, float>();
            if (res == null) return dict;

            // This would need actual GameRes field access
            // Placeholder implementation
            return dict;
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
