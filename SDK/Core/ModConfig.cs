using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using GraveSDK.Data.Models;

namespace GraveSDK.Core
{
    [Serializable]
    public class ModConfig
    {
        // ============================================================
        // Core Features (Serialized)
        // ============================================================
        public bool ModEnabled = true;
        public bool EnableGodMode = false;
        public bool InfiniteEnergy = false;
        public bool UnlockAllCrafts = false;
        public bool UseCustomCosts = false;
        public bool ShowItemMetadata = true;
        public bool ShowExtendedTooltips = false;
        public bool DevMode = false;
        
        public float MovementSpeedMultiplier = 1.0f;
        public float GlobalCraftSpeedMultiplier = 1.0f;
        public float GlobalCraftQualityBonus = 0.0f;
        
        public KeyCode ToggleMenuKey = KeyCode.F1;

        // ============================================================
        // Runtime Modifications (Not Serialized by JsonUtility)
        // ============================================================
        public Dictionary<string, string> ItemNameOverrides = new Dictionary<string, string>();
        public Dictionary<string, string> ItemPrefixes = new Dictionary<string, string>();
        public Dictionary<string, string> ItemSuffixes = new Dictionary<string, string>();
        public Dictionary<string, string> CustomItemDescriptions = new Dictionary<string, string>();
        public Dictionary<string, List<string>> ExtendedTooltipSections = new Dictionary<string, List<string>>();
        
        public Dictionary<string, IUnlockCondition> CustomUnlockConditions = new Dictionary<string, IUnlockCondition>();
        public Dictionary<string, ICraftCostCalculator> CustomCraftCosts = new Dictionary<string, ICraftCostCalculator>();
        public Dictionary<string, float> CustomCraftTimes = new Dictionary<string, float>();
        public Dictionary<string, float> CraftDiscounts = new Dictionary<string, float>();
        public Dictionary<string, float> CraftQualityBonus = new Dictionary<string, float>();
        
        public HashSet<string> ForceSingleCraft = new HashSet<string>();
        public HashSet<string> ForceMultiCraft = new HashSet<string>();
        public HashSet<CraftType> AutoUnlockCraftTypes = new HashSet<CraftType>();
        
        public Dictionary<string, string> CraftNamePrefixes = new Dictionary<string, string>();
        public Dictionary<string, string> CraftNameSuffixes = new Dictionary<string, string>();
    }

    public static class ConfigManager
    {
        private static string ConfigPath => Path.Combine(Application.persistentDataPath, "GraveSDK_Config.json");
        public static ModConfig Current { get; private set; } = new ModConfig();

        public static void Load()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    string json = File.ReadAllText(ConfigPath);
                    // JsonUtility only handles simple types. 
                    // Dictionaries and HashSets will remain as initialized in constructor.
                    JsonUtility.FromJsonOverwrite(json, Current);
                    Debug.Log("[GraveSDK] Config loaded from: " + ConfigPath);
                }
                else
                {
                    Save(); // Create default config
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("[GraveSDK] Failed to load config: " + ex.Message);
            }
        }

        public static void Save()
        {
            try
            {
                string json = JsonUtility.ToJson(Current, true);
                File.WriteAllText(ConfigPath, json);
                Debug.Log("[GraveSDK] Config saved to: " + ConfigPath);
            }
            catch (Exception ex)
            {
                Debug.LogError("[GraveSDK] Failed to save config: " + ex.Message);
            }
        }
    }
}
