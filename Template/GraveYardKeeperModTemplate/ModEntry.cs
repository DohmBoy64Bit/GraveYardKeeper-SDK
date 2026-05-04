using System;
using UnityEngine;
using GraveSDK.Core;
using GraveSDK.Data.Repositories;
using GraveSDK.Data.Models;
using System.Collections.Generic;
using System.Linq;

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

            // 4. Vendor Injection
            var vendorRepo = new VendorRepository();
            vendorRepo.AddItemToTrade("gerry", "baked_potato", 1, 5); // Add potatoes to Gerry
            Debug.Log("[ExampleMod] Injected baked_potato into Gerry's shop.");
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
        private NPCRepository _npc = new NPCRepository();

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

            // H: Make Player Say Something
            if (Input.GetKeyDown(KeyCode.H))
            {
                DialogueRepository.SayAsPlayer("Hello, world! I am modded.");
            }

            // J: Show Choice Dialog
            if (Input.GetKeyDown(KeyCode.J))
            {
                ShowExampleChoices();
            }

            // U: Screen Shake
            if (Input.GetKeyDown(KeyCode.U))
            {
                CameraRepository.Shake(2f, 0.5f);
            }

            // I: Toggle Cinematic Filter
            if (Input.GetKeyDown(KeyCode.I))
            {
                _isFilterOn = !_isFilterOn;
                CameraRepository.SetFilter("CameraFilterPack_TV_Old", _isFilterOn);
                _ui.ShowNotification(_isFilterOn ? "Old TV Filter: ON" : "Old TV Filter: OFF");
            }

            // Y: Boost Zombie Efficiency (if looking at a zombie)
            if (Input.GetKeyDown(KeyCode.Y))
            {
                BoostNearZombie();
            }
        }

        private bool _isFilterOn = false;

        private void ShowExampleChoices()
        {
            var options = new List<string> { "I am a friend", "I am a foe", "I am a potato" };
            var gerry = _world.GetNPC("gerry");
            if (gerry != null)
            {
                DialogueRepository.ShowOptions(gerry, options, (chosen) => {
                    DialogueRepository.SayAsPlayer("I chose: " + chosen);
                    if (chosen == "I am a potato")
                    {
                        CameraRepository.Shake(5f, 1f);
                        _ui.ShowMessage("A potato?! PREPOSTEROUS!");
                    }
                });
            }
        }

        private void BoostNearZombie()
        {
            var playerPos = _player.GetPosition();
            var zombies = _world.GetAllObjects()
                .Where(w => w.obj_id.StartsWith("worker_zombie") && Vector3.Distance(w.transform.position, playerPos) < 5f)
                .ToList();

            foreach (var zombie in zombies)
            {
                WorkerRepository.SetEfficiency(zombie, 2.0f); // 200% efficiency!
                DialogueRepository.Say(zombie, "UNLIMITED POWER!", SpeechBubbleGUI.SpeechBubbleType.Talk);
            }

            if (zombies.Count > 0) _ui.ShowNotification("Zombies boosted to 200%!");
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
