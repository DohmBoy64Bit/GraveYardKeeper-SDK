using System;
using UnityEngine;
using GraveSDK.Core;
using GraveSDK.Data.Repositories;
using GraveSDK.Data.Models;
using System.Collections.Generic;

namespace ExampleMod
{
    /// <summary>
    /// ExampleMod demonstrates the comprehensive use of the GraveYardKeeper SDK.
    /// This template covers Repositories, Hooks, Configuration, and UI features.
    /// </summary>
    public static class ModEntry
    {
        // Define repositories at the class level for easy access
        private static PlayerRepository _playerRepo = new PlayerRepository();
        private static ItemRepository _itemRepo = new ItemRepository();
        private static InventoryRepository _invRepo = new InventoryRepository();
        private static CraftRepository _craftRepo = new CraftRepository();
        private static WorldRepository _worldRepo = new WorldRepository();
        private static UIRepository _uiRepo = new UIRepository();
        private static EnvironmentRepository _envRepo = new EnvironmentRepository();
        private static QuestRepository _questRepo = new QuestRepository();
        private static BuffRepository _buffRepo = new BuffRepository();
        private static SaveRepository _saveRepo = new SaveRepository();
        private static NPCRepository _npcRepo = new NPCRepository();

        /// <summary>
        /// The SDK ModLoader looks for a static 'Initialize' method in your DLL.
        /// Use this to set up your mod logic and register hooks.
        /// </summary>
        public static void Initialize()
        {
            Debug.Log("[ExampleMod] Initializing Comprehensive Template...");
            
            // 1. Configure SDK behaviors via ConfigManager
            SetupSDKConfiguration();

            // 2. Demonstrate Data Querying (Repositories)
            DemonstrateDataAccess();

            // 3. Register a controller to handle runtime logic
            var go = new GameObject("ExampleModController");
            go.AddComponent<ModController>();
            UnityEngine.Object.DontDestroyOnLoad(go);
            
            Debug.Log("[ExampleMod] Initialization Complete. Use K/L/O/P keys in-game.");
        }

        private static void SetupSDKConfiguration()
        {
            // SDK Configuration allows you to tweak global game behaviors easily
            ConfigManager.Current.MovementSpeedMultiplier = 1.3f; // Slightly faster movement
            ConfigManager.Current.ShowItemMetadata = true;       // Show internal IDs in tooltips
            ConfigManager.Current.UnlockAllCrafts = false;      // Keep balance for now
            
            // Register custom descriptions for specific items
            ConfigManager.Current.CustomItemDescriptions["wood_stick"] = "A stick modified by ExampleMod! [color=#00ff00]Quality: Excellent[/color]";
            
            // Add custom name modifications
            ConfigManager.Current.ItemPrefixes["stone"] = "[Reinforced] ";
            ConfigManager.Current.CraftNameSuffixes["craft_stone_block"] = " (Expertly Cut)";
            
            // Auto-unlock specific categories if desired
            ConfigManager.Current.AutoUnlockCraftTypes.Add(GraveSDK.Data.Models.CraftType.AlchemyDecompose);
        }

        private static void DemonstrateDataAccess()
        {
            // Use Repositories to browse game data without directly touching GameBalance
            
            // 1. Item Data
            var stone = _itemRepo.GetItem("stone");
            if (stone != null) Debug.Log($"[ExampleMod] Item: {stone.DisplayName}, Base Price: {stone.BasePrice}");

            // 2. Craft Data
            var craft = _craftRepo.GetCraft("craft_stone_block");
            if (craft != null) Debug.Log($"[ExampleMod] Craft: {craft.DisplayName}, Time: {craft.TimeCost}");

            // 3. Quest Status
            var quests = _questRepo.GetAllQuests();
            Debug.Log($"[ExampleMod] Tracking {quests.Count} total quests.");

            // 4. Buff Information
            var buffs = _buffRepo.GetAllBuffs();
            Debug.Log($"[ExampleMod] Total Buffs available: {buffs.Count}");
        }
    }

    /// <summary>
    /// ModController handles per-frame logic and user input.
    /// </summary>
    public class ModController : MonoBehaviour
    {
        private PlayerRepository _player = new PlayerRepository();
        private EnvironmentRepository _env = new EnvironmentRepository();
        private UIRepository _ui = new UIRepository();
        private WorldRepository _world = new WorldRepository();
        private InventoryRepository _inv = new InventoryRepository();
        private SaveRepository _save = new SaveRepository();

        void Update()
        {
            // K: Grant Starter Kit
            if (Input.GetKeyDown(KeyCode.K))
            {
                GrantStarterKit();
            }

            // L: Toggle Day/Night
            if (Input.GetKeyDown(KeyCode.L))
            {
                ToggleDayNight();
            }
            
            // O: Show Notification
            if (Input.GetKeyDown(KeyCode.O))
            {
                _ui.ShowNotification("Mod Template Notification!");
            }

            // P: Teleport to Gerry (if Gerry is spawned)
            if (Input.GetKeyDown(KeyCode.P))
            {
                TeleportToGerry();
            }
        }

        private void GrantStarterKit()
        {
            // Using InventoryRepository to safely add items
            _inv.AddItem("wood_stick", 10);
            _inv.AddItem("stone", 10);
            _inv.AddItem("baked_potato", 3);
            
            // Use UIRepository for user feedback
            _ui.ShowMessage("ExampleMod: Starter kit delivered!");
            
            // Use PlayerRepository for money and tech
            _player.AddMoney(100000); // 10 Gold (100,000 copper)
            _player.AddTechPoints(50, 50, 50); // Red, Green, Blue points
        }

        private void ToggleDayNight()
        {
            var state = _env.GetState();
            if (state.IsNight)
            {
                _env.SetTime(0.15f); // 6:00 AM
                _ui.ShowNotification("Good Morning!");
            }
            else
            {
                _env.SetTime(0.85f); // 10:00 PM
                _ui.ShowNotification("Night Falls...");
            }
        }

        private void TeleportToGerry()
        {
            // Using WorldRepository to find an NPC
            var gerry = _world.GetNPC("gerry");
            if (gerry != null)
            {
                _player.TeleportTo(gerry.transform.position + Vector3.right * 2f);
                _ui.ShowNotification("Teleported near Gerry!");
            }
            else
            {
                _ui.ShowNotification("Gerry not found in current scene.");
            }
        }
        
        private void OnDestroy()
        {
            // Save mod settings when the controller is destroyed (e.g., game close)
            ConfigManager.Save();
        }
    }
}
