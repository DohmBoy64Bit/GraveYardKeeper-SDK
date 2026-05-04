using System;
using System.Collections.Generic;
using UnityEngine;
using GraveSDK.Data.Models;

namespace GraveSDK.Utils
{
    /// <summary>
    /// Central mod configuration
    /// Stores all mod settings and options
    /// </summary>
    public static class ModConfig
    {
        // ============================================================
        // Core Features
        // ============================================================

        /// <summary>
        /// Enable/disable all mod functionality
        /// </summary>
        public static bool ModEnabled { get; set; } = true;

        /// <summary>
        /// Unlock all crafts automatically
        /// </summary>
        public static bool UnlockAllCrafts { get; set; } = false;

        /// <summary>
        /// Show extra item information in tooltips
        /// </summary>
        public static bool ShowItemMetadata { get; set; } = true;

        /// <summary>
        /// Show extended tooltips with debug info
        /// </summary>
        public static bool ShowExtendedTooltips { get; set; } = false;

        /// <summary>
        /// Enable developer mode (debug features)
        /// </summary>
        public static bool DevMode { get; set; } = false;

        // ============================================================
        // Crafting Modifications
        // ============================================================

        /// <summary>
        /// Global craft speed multiplier
        /// </summary>
        public static float GlobalCraftSpeedMultiplier { get; set; } = 1f;

        /// <summary>
        /// Global craft quality bonus
        /// </summary>
        public static float GlobalCraftQualityBonus { get; set; } = 0f;

        /// <summary>
        /// Use custom craft costs
        /// </summary>
        public static bool UseCustomCosts { get; set; } = false;

        /// <summary>
        /// Auto-unlock specific craft types
        /// </summary>
        public static HashSet<CraftType> AutoUnlockCraftTypes { get; set; } = new HashSet<CraftType>();

        // ============================================================
        // Item Modifications
        // ============================================================

        /// <summary>
        /// Custom item name overrides
        /// Key: item ID, Value: custom name
        /// </summary>
        public static Dictionary<string, string> ItemNameOverrides { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Item name prefixes
        /// Key: item ID, Value: prefix text
        /// </summary>
        public static Dictionary<string, string> ItemPrefixes { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Item name suffixes
        /// Key: item ID, Value: suffix text
        /// </summary>
        public static Dictionary<string, string> ItemSuffixes { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Custom item descriptions
        /// Key: item ID, Value: custom description
        /// </summary>
        public static Dictionary<string, string> CustomItemDescriptions { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Extended tooltip sections
        /// Key: item ID, Value: list of tooltip lines
        /// </summary>
        public static Dictionary<string, List<string>> ExtendedTooltipSections { get; set; } = new Dictionary<string, List<string>>();

        // ============================================================
        // Craft Customizations
        // ============================================================

        /// <summary>
        /// Custom unlock conditions for crafts
        /// Key: craft ID, Value: unlock condition
        /// </summary>
        public static Dictionary<string, IUnlockCondition> CustomUnlockConditions { get; set; } = new Dictionary<string, IUnlockCondition>();

        /// <summary>
        /// Custom craft costs
        /// Key: craft ID, Value: custom cost calculator
        /// </summary>
        public static Dictionary<string, ICraftCostCalculator> CustomCraftCosts { get; set; } = new Dictionary<string, ICraftCostCalculator>();

        /// <summary>
        /// Custom craft times (in seconds)
        /// Key: craft ID, Value: time in seconds
        /// </summary>
        public static Dictionary<string, float> CustomCraftTimes { get; set; } = new Dictionary<string, float>();

        /// <summary>
        /// Craft discounts (percentage)
        /// Key: craft ID, Value: discount percentage (0-100)
        /// </summary>
        public static Dictionary<string, float> CraftDiscounts { get; set; } = new Dictionary<string, float>();

        /// <summary>
        /// Craft quality bonuses
        /// Key: craft ID, Value: quality bonus
        /// </summary>
        public static Dictionary<string, float> CraftQualityBonus { get; set; } = new Dictionary<string, float>();

        /// <summary>
        /// Crafts that must be single-craft only
        /// </summary>
        public static HashSet<string> ForceSingleCraft { get; set; } = new HashSet<string>();

        /// <summary>
        /// Crafts that can always be multi-craft
        /// </summary>
        public static HashSet<string> ForceMultiCraft { get; set; } = new HashSet<string>();

        /// <summary>
        /// Craft name prefixes
        /// Key: craft ID, Value: prefix text
        /// </summary>
        public static Dictionary<string, string> CraftNamePrefixes { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Craft name suffixes
        /// Key: craft ID, Value: suffix text
        /// </summary>
        public static Dictionary<string, string> CraftNameSuffixes { get; set; } = new Dictionary<string, string>();

        // ============================================================
        // UI Configuration
        // ============================================================

        /// <summary>
        /// Mod menu hotkey
        /// </summary>
        public static KeyCode MenuHotkey { get; set; } = KeyCode.F1;

        /// <summary>
        /// Show console window
        /// </summary>
        public static bool ShowConsole { get; set; } = false;

        /// <summary>
        /// Auto-save configuration interval (seconds)
        /// </summary>
        public static float AutoSaveInterval { get; set; } = 300f;

        // ============================================================
        // Localization
        // ============================================================

        /// <summary>
        /// Force specific language
        /// </summary>
        public static string ForceLanguage { get; set; } = null;

        /// <summary>
        /// Custom translations
        /// Key: localization key, Value: custom translation
        /// </summary>
        public static Dictionary<string, string> CustomTranslations { get; set; } = new Dictionary<string, string>();

        // ============================================================
        // Hotkey Methods
        // ============================================================

        /// <summary>
        /// Check if menu hotkey is pressed
        /// </summary>
        public static bool IsHotkeyPressed()
        {
            return UnityEngine.Input.GetKeyDown(MenuHotkey);
        }

        /// <summary>
        /// Setup hotkey from configuration
        /// </summary>
        public static void SetupHotkey()
        {
            // Load from config file if exists
            // Implementation depends on BepInEx config system
        }

        // ============================================================
        // Configuration Management
        // ============================================================

        /// <summary>
        /// Save configuration to file
        /// </summary>
        public static void Save()
        {
            // Implementation using BepInEx Config
        }

        /// <summary>
        /// Load configuration from file
        /// </summary>
        public static void Load()
        {
            // Implementation using BepInEx Config
        }

        /// <summary>
        /// Reset to defaults
        /// </summary>
        public static void Reset()
        {
            ModEnabled = true;
            UnlockAllCrafts = false;
            ShowItemMetadata = true;
            ShowExtendedTooltips = false;
            DevMode = false;
            GlobalCraftSpeedMultiplier = 1f;
            GlobalCraftQualityBonus = 0f;
            UseCustomCosts = false;
            AutoUnlockCraftTypes.Clear();
            ItemNameOverrides.Clear();
            ItemPrefixes.Clear();
            ItemSuffixes.Clear();
            CustomItemDescriptions.Clear();
            ExtendedTooltipSections.Clear();
            CustomUnlockConditions.Clear();
            CustomCraftCosts.Clear();
            CustomCraftTimes.Clear();
            CraftDiscounts.Clear();
            CraftQualityBonus.Clear();
            ForceSingleCraft.Clear();
            ForceMultiCraft.Clear();
            CraftNamePrefixes.Clear();
            CraftNameSuffixes.Clear();
            MenuHotkey = KeyCode.F1;
            ShowConsole = false;
            AutoSaveInterval = 300f;
            ForceLanguage = null;
            CustomTranslations.Clear();
        }
    }

    /// <summary>
    /// Interface for custom unlock conditions
    /// </summary>
    public interface IUnlockCondition
    {
        bool IsUnlocked();
        string GetDescription();
    }

    /// <summary>
    /// Interface for custom craft cost calculators
    /// </summary>
    public interface ICraftCostCalculator
    {
        string GetDisplayText(WorldGameObject wgo, int multiplier);
        bool CanCraft(WorldGameObject wgo);
    }

    /// <summary>
    /// Simple unlock condition based on perk
    /// </summary>
    public class PerkUnlockCondition : IUnlockCondition
    {
        public string PerkId { get; set; }

        public bool IsUnlocked()
        {
            return MainGame.me?.save?.unlocked_perks?.Contains(PerkId) ?? false;
        }

        public string GetDescription()
        {
            return $"Requires perk: {PerkId}";
        }
    }

    /// <summary>
    /// Simple unlock condition based on game flag
    /// </summary>
    public class FlagUnlockCondition : IUnlockCondition
    {
        public string FlagId { get; set; }
        public bool RequiredValue { get; set; } = true;

        public bool IsUnlocked()
        {
            if (MainGame.me?.player == null) return false;
            bool hasFlag = MainGame.me.player.GetParamInt(FlagId) > 0;
            return hasFlag == RequiredValue;
        }

        public string GetDescription()
        {
            return $"Requires flag '{FlagId}' = {RequiredValue}";
        }
    }

    /// <summary>
    /// Simple craft cost calculator
    /// </summary>
    public class SimpleCraftCost : ICraftCostCalculator
    {
        public float EnergyCost { get; set; }
        public float TimeCost { get; set; }

        public string GetDisplayText(WorldGameObject wgo, int multiplier)
        {
            return $"[c](en)[/c]{EnergyCost * multiplier}\n[c](time)[/c]{TimeCost * multiplier:0.##}";
        }

        public bool CanCraft(WorldGameObject wgo)
        {
            // Check if player has enough energy
            return true;
        }
    }
}
