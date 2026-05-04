# Graveyard Keeper SDK Architecture & Code Changes

## Last Updated
2026-05-04 (v1.3 Reactive Framework)

## Overview
This document describes the proposed C# SDK/Mod Menu foundation architecture, built using Separation of Concerns (SoC) principles. All code is designed as wrapper classes and Harmony patches - **never modifying base game code directly**.

---

## Repository Structure
```
GraveYardKeeper-SDK/
├── docs/
│   ├── Wiki_Findings.md       # Extracted game data
│   └── Code_Changes.md        # This file
├── SDK/
│   ├── Core/
│   │   ├── Plugin.cs          # BepInEx Entry Point
│   │   ├── ConfigManager.cs   # Persistent Configuration
│   │   └── ModLoader.cs       # External Mod Loading
│   ├── Hooks/
│   │   ├── Inventory/
│   │   ├── Crafting/
│   │   ├── UI/
│   │   ├── Localization/
│   │   └── Engine/
│   ├── Data/
│   │   ├── Models/
│   │   └── Repositories/
│   ├── UI/
│   │   ├── Components/
│   │   └── ModMenu/
│   └── Utils/
│       └── Extensions/
├── Template/                  # Mod Template Project
│   └── GraveYardKeeperModTemplate/
└── README.md
```

---

## 1. Data Models (SoC Layer 1)

### 1.1 SDK Data Models
**Location:** `SDK/Data/Models/`

#### ItemModel.cs
```csharp
namespace GraveSDK.Data.Models
{
    public class ItemModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public ItemType Type { get; set; }
        public float Quality { get; set; }
        public int StackCount { get; set; }
        public float Durability { get; set; }
        public bool HasDurability { get; set; }
        public Dictionary<string, float> Parameters { get; set; }
        public List<string> OnUseEffects { get; set; }
    }
}
```

#### CraftModel.cs
```csharp
namespace GraveSDK.Data.Models
{
    public class CraftModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public List<CraftIngredient> Needs { get; set; }
        public List<CraftResult> Output { get; set; }
        public float Difficulty { get; set; }
        public List<string> RequiredPerks { get; set; }
        public bool IsLocked { get; set; }
        public bool CanAutoCraft { get; set; }
        public float EnergyCost { get; set; }
        public float TimeCost { get; set; }
    }
}
```

#### BuffModel.cs
```csharp
namespace GraveSDK.Data.Models
{
    public class BuffModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public float Duration { get; set; }
        public bool IsHidden { get; set; }
        public Dictionary<string, float> ResourceEffects { get; set; }
        public string CustomIcon { get; set; }
    }
}
```

---

## 2. Game Data Registries

The game stores its master database in a `GameBalance` ScriptableObject (loaded from `Resources/game_data`) containing lists of definitions for almost every entity in the game.

**Location:** `Assembly-CSharp/GameBalance.cs` - Token: 0x02000369 RID: 873

### Key Data Lists
```csharp
// Balance data registries
public List<ProductTypeDefinition> product_types_data;
public List<PerkDefinition> perks_data;
public List<ItemDefinition> items_data;
public List<ObjectDefinition> objs_data;
public List<BuffDefinition> buffs_data;
public List<VendorDefinition> vendors_data;
public List<CraftDefinition> craft_data;        // Includes standard, survey, alchemy, sermon
public List<ObjectCraftDefinition> craft_obj_data; // Building/removal definitions
public List<QuestDefinition> quests_data;
public List<TechDefinition> techs_data;
public List<TechBranchDefinition> tech_branches_data;
public List<SoulDefinition> souls_data;
public List<BodyDefinition> bodies_data;
public List<FishDefinition> fishes_data;
// ... and 20+ more specialized lists
```

### Data Loading
```csharp
public static void LoadGameBalance()
```
- Loads `game_data` from Resources
- Calls: `CreateIDsCache()`, `CreateItemsBaseNameCache()`, `CreateToolsCache()`, `CreateCraftsCache()`
- **Hook Point:** Postfix allows injecting/modifying definitions before caches are built

### Singleton Access
```csharp
public static GameBalance me  // Auto-loads if null
```

### Hardcoded Initialization
```csharp
// In MainGame.GeneralInit() - automatically added on game start
this.save.obj_crafts.AddCraft("p:grave_place");
this.save.obj_crafts.AddCraft("p:working_table");
```

---

## 3. Key Functions & Hook Points

### MainGame.cs - Token: 0x02000497 RID: 1175

#### `public void GeneralInit()` (line 113)
Core initialization sequence:
- Sets up engine managers (LazyEngine, ChunkManager, etc.)
- **Calls `GameBalance.LoadGameBalance()`** at line 133
- Adds hardcoded starting crafts ("p:grave_place", "p:working_table")
- Initializes GUI, tech linking, camera, fog
- **Hook Point:** Postfix for post-initialization logic

#### `private void InGameUpdate()` (line 259)
Main logic tick - called each frame:
```csharp
this.save.game_logics.Update();          // Game logic state
BuffsLogics.RecalculateBuffs();          // Status effect updates
this.build_mode_logics.Update();         // Building mechanics
UtilityStuff.ProcessInGameUpdate();      // Utility systems
```
- **Hook Point:** Postfix for per-frame custom logic (when unpaused)
- Prime target for custom tick-based systems

#### `public void OnPlayerDied()` (line 302)
Handles player death sequence:
```csharp
// Determines respawn location
WorldMap.GetGDPointByGDTag("gd_player_respawn", true, true);

// Resets player position
this.player.transform.position = target.transform.position;

// Reduces character stats by 10%
this.player.SubParam("_r", num);  // Red parameter
this.player.SubParam("_g", num);  // Green parameter  
this.player.SubParam("_b", num);  // Blue parameter
```
- **Hook Point:** Prefix/Postfix for death penalty modifications, respawn location changes
- Can prevent stat loss, modify respawn behavior, add death effects

#### Other Key Fields
```csharp
public static MainGame me;              // Singleton instance
public WorldGameObject player;          // Player reference  
public GameSave save;                   // Current game save
public static bool paused;              // Game pause state
public static bool game_started;        // Initialization flag
public static float game_time;          // Current game time
```

---

## 4. SDK Architecture & SoC Integration

### Proposed Hook Implementation
```csharp
// SDK/Hooks/Data/GameBalancePatch.cs
[HarmonyPatch(typeof(GameBalance))]
[HarmonyPatch("LoadGameBalance")]
public static class GameBalanceLoadPatch
{
    [HarmonyPostfix]
    public static void LoadGameBalance_Postfix()
    {
        // Inject custom definitions after base data loads
        foreach (var customItem in CustomContentRegistry.Items)
        {
            GameBalance.me.items_data.Add(customItem);
        }
        
        // Rebuild caches with new data
        GameBalance.me.CreateIDsCache();
        GameBalance.me.CreateItemsBaseNameCache();
    }
}

// SDK/Hooks/Gameplay/PlayerDeathPatch.cs
[HarmonyPatch(typeof(MainGame))]
[HarmonyPatch("OnPlayerDied")]
public static class PlayerDeathPatch
{
    [HarmonyPrefix]
    public static bool OnPlayerDied_Prefix()
    {
        if (ModConfig.PreventStatLoss)
        {
            // Custom death handler - prevent stat reduction
            return false;  // Skip original method
        }
        return true;  // Run original method
    }
}
```

### DataRegistry Wrapper Class
```csharp
// SDK/Data/Repositories/CustomContentRegistry.cs
public static class CustomContentRegistry
{
    public static List<ItemDefinition> Items { get; } = new();
    public static List<CraftDefinition> Crafts { get; } = new();
    
    public static void RegisterItem(ItemDefinition item)
    {
        Items.Add(item);
    }
    
    public static void RegisterCraft(CraftDefinition craft)
    {
        Crafts.Add(craft);
    }
}
```

---

## 5. Modding Concepts

### Custom Item Injector
By hooking `GameBalance.LoadGameBalance()`, modders can insert new `ItemDefinition` and `CraftDefinition` instances into the native lists, enabling fully custom items and recipes without modifying `game_data`.

**Implementation:**
1. Create custom `ItemDefinition` instance  
2. Register with `CustomContentRegistry.RegisterItem()`
3. Postfix `LoadGameBalance()` adds to `GameBalance.me.items_data`
4. Game treats it like any other item

### Death Penalty Tweaker
Hooking `MainGame.OnPlayerDied()` allows modders to:
- Prevent stat/resource loss on death
- Change respawn location
- Add custom death effects
- Implement permadeath or hardcore modes

### Time Scale Mod
Hooking `MainGame.InGameUpdate()` or overriding `MainGame.game_time` enables:
- Day/night cycle speed changes
- Pausing game time
- Time-based events
- Slow-motion effects

---

## 6. Repository Documentation Updates

### Append to `docs/Wiki_Findings.md`:

```markdown
## Core Data Structures

The game stores its master database in a `GameBalance` ScriptableObject, loaded from `Resources/game_data`. 
It contains `List<T>` for all definitions (Items, Objects, Crafts, Techs, Quests, NPCs).

*   **Singleton:** `GameBalance.me`
*   **Key Lists:** `items_data`, `objs_data`, `craft_data`, `craft_obj_data`, `techs_data`
*   **Hardcoded Starting Crafts:** `"p:grave_place"`, `"p:working_table"`
```

### Append to `docs/Code_Changes.md`:

```markdown
## Proposed Hooks & Architecture

### Harmony Hooks

1.  **Target:** `GameBalance.LoadGameBalance()`
    *   **Type:** Postfix
    *   **Purpose:** Inject custom `ItemDefinition`, `CraftDefinition`, etc. into the `GameBalance.me` lists immediately after the base data loads, but *before* internal dictionaries are fully populated.

2.  **Target:** `MainGame.OnPlayerDied()`
    *   **Type:** Prefix/Postfix
    *   **Purpose:** Intercept or alter death penalties, respawn locations, and UI popups.

3.  **Target:** `MainGame.InGameUpdate()`
    *   **Type:** Postfix
    *   **Purpose:** Safe injection point for per-frame custom mod logic that requires the game to be unpaused.

### SDK Implementation

*   Create a `DataRegistry` wrapper class. Modders call `DataRegistry.RegisterItem(myCustomItemDef)`.
*   The `GameBalance.LoadGameBalance()` hook iterates through the `DataRegistry` and appends all queued custom definitions into the native `GameBalance` lists.
```

---

## 3. Wrapper Classes (SoC Layer 2)

### 2.1 Item Wrapper
**Location:** `SDK/Data/Repositories/ItemRepository.cs`
```csharp
namespace GraveSDK.Data.Repositories
{
    public class ItemRepository
    {
        private readonly Dictionary<string, ItemModel> _itemCache;
        
        public ItemModel GetItem(string itemId)
        {
            // Wrapper around ItemDefinition
            ItemDefinition def = GameBalance.me.GetData<ItemDefinition>(itemId);
            if (def == null) return null;
            
            return new ItemModel
            {
                Id = def.id,
                DisplayName = def.GetItemName(true),
                Description = def.GetItemDescription(),
                Type = (GraveSDK.Data.Models.ItemType)def.type,
                Quality = def.quality,
                StackCount = def.stack_count,
                HasDurability = def.has_durability,
                Durability = def.durability_decrease,
                Parameters = ConvertGameRes(def.parameters),
                OnUseEffects = def.on_use_expressions
                    .Select(e => e.GetRawExpressionString())
                    .ToList()
            };
        }
        
        public List<ItemModel> GetAllItems()
        {
            if (GameBalance.me?.items_data == null)
                return new List<ItemModel>();

            return GameBalance.me.items_data
                .Select(d => GetItem(d.id))
                .Where(i => i != null)
                .ToList();
        }
        
        public List<ItemModel> GetItemsByType(ItemType type)
        {
            return GetAllItems().Where(i => i.Type == type).ToList();
        }
    }
}
```

### 2.2 Craft Repository
**Location:** `SDK/Data/Repositories/CraftRepository.cs`
Includes a safety prefix hook on `GetNameNonLocalized` to prevent `IndexOutOfRangeException` caused by malformed Craft IDs in the base game.

### 2.3 Buff Repository
**Location:** `SDK/Data/Repositories/BuffRepository.cs`
Hardened with try-catch blocks for `SmartExpression` evaluation and null-safe localization fallbacks. This prevents initialization crashes when game data is accessed before the player context is fully ready.

### 2.3 Localization Repository
**Location:** `SDK/Data/Repositories/LocalizationRepository.cs`
High-level wrapper for the game's `Localization` class, supporting safe key retrieval and language switching.

### 2.4 World Repository
**Location:** `SDK/Data/Repositories/WorldRepository.cs`
Handles entity spawning, NPC finding, and world object search by tag or ID.

### 2.5 Inventory Repository
**Location:** `SDK/Data/Repositories/InventoryRepository.cs`
Unified interface for player bags, equipped items, and nearest chest/container access.

### 2.6 Save Repository
**Location:** `SDK/Data/Repositories/SaveRepository.cs`
Handles manual persistence, tech/craft unlocking, and custom game flag management.

### 2.7 Player Repository
**Location:** `SDK/Data/Repositories/PlayerRepository.cs`
Direct access to player stats (Health, Energy, Money, Tech Points) and teleportation.

### 2.8 Registry Repository
**Location:** `SDK/Data/Repositories/RegistryRepository.cs`
High-level API for modders to inject custom `ItemDefinition` and `CraftDefinition` into the game.

### 2.9 UI Repository
**Location:** `SDK/Data/Repositories/UIRepository.cs`
Window management, dialog popups, and HUD control.

### 2.10 NPC & Quest Repositories
**Location:** `SDK/Data/Repositories/NPCRepository.cs`, `QuestRepository.cs`
Read-only access to NPC metadata and quest progress tracking.

### 2.11 Perk & Tech Repositories
**Location:** `SDK/Data/Repositories/PerkRepository.cs`, `TechRepository.cs`
Management of player perks and technology tree nodes.

### 2.12 Dialogue Repository
**Location:** `SDK/Data/Repositories/DialogueRepository.cs`
High-level API for NPC/Player speech, multi-choice dialogue options, and cinematic notifications.

### 2.13 Camera Repository
**Location:** `SDK/Data/Repositories/CameraRepository.cs`
Controls cinematic effects including screen shake, fades, letterboxing, and 100+ built-in `CameraFilterPack` filters.

### 2.14 Worker Repository
**Location:** `SDK/Data/Repositories/WorkerRepository.cs`
Specialized management for zombies and workers, including efficiency adjustment and item/WGO transformation.

### 2.15 Interaction Repository
**Location:** `SDK/Data/Repositories/InteractionRepository.cs`
Custom interaction system allowing modders to add/remove interaction events on WGOs, fire game events, and register custom callbacks.

### 2.16 Game Event Repository
**Location:** `SDK/Data/Repositories/GameEventRepository.cs`
Reactive event bus providing C# events for game lifecycle (OnDayChanged, OnPlayerInteract, OnCraftCompleted, OnGameSaved, OnPlayerDied, OnGameTick).

### 2.17 Economy Repository
**Location:** `SDK/Data/Repositories/EconomyRepository.cs`
Full vendor/economy management including creating new VendorDefinitions, manipulating vendor tiers, money, inventory, and global item pricing.

### 2.18 SmartExpression Repository & QuestBuilder
**Location:** `SDK/Data/Repositories/SmartExpressionRepository.cs`
Exposes the game's built-in SmartExpression scripting system for modders. Includes:
- **SmartExpressionRepository**: Create, evaluate, and execute SmartExpression strings (e.g., `Ppar("money")`, `SetPpar("hp", 100)`, `GetDay() > 10`)
- **QuestBuilder**: Builder pattern for creating QuestDefinitions with custom SmartExpression triggers:
  - `SetStartTrigger(expr)` — condition for quest auto-start
  - `SetSuccessTrigger(expr)` — condition for quest completion (e.g., `"Ppar(\"slimes_killed\") >= 5"`)
  - `SetFailTrigger(expr)` — condition for quest failure (e.g., `"GetDay() > 20"`)
  - `SetStartKey(keys)` — game event keys that trigger quest start (e.g., `"interact_gerry"`, `"end_of_day"`)
  - `AddSuccessExpression(expr)` — side-effects on success (e.g., `"AddPpar(\"money\", 5000)"`)
  - `BuildAndRegister()` — builds and injects the quest into GameBalance

---

## 6. SDK Lifecycle & Entry Point

### 6.1 BepInEx Plugin (Plugin.cs)
**Location:** `SDK/Core/Plugin.cs`
The main entry point for the SDK. It initializes the Harmony patches and the Mod Menu.

```csharp
[BepInPlugin("com.gravesdk.core", "GraveYardKeeperSDK", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    void Awake()
    {
        // Load persistent configuration
        ConfigManager.Load();
        
        // Initialize Harmony
        var harmony = new Harmony("com.gravesdk.core");
        harmony.PatchAll();
    }
}
```

### 6.2 Harmony Patching Standards
To avoid `AmbiguousMatchException` in Unity's Mono environment, all patches must specify parameter types when targeting overloaded methods.

**Example:**
```csharp
[HarmonyPatch(typeof(UIRoot))]
[HarmonyPatch("Broadcast", new System.Type[] { typeof(string) })]
public static class UIRootPatch { ... }
```

---

## 7. Configuration System

### 7.1 ConfigManager
**Location:** `SDK/Core/ModConfig.cs`
Handles JSON serialization of `ModConfig`. This ensures settings persist across game restarts and can be edited via the Mod Menu.

---

## 3. Harmony Hook Patches

### 3.1 Item Usage Hook
**Location:** `SDK/Hooks/Inventory/ItemUsePatch.cs`
```csharp
using HarmonyLib;
using GraveSDK.Data.Repositories;

namespace GraveSDK.Hooks.Inventory
{
    [HarmonyPatch(typeof(ItemDefinition))]
    [HarmonyPatch("GetItemDescription")]
    public static class ItemUsePatch
    {
        private static readonly ItemRepository _repo = new ItemRepository();
        
        [HarmonyPostfix]
        public static void GetItemDescription_Postfix(
            ItemDefinition __instance,
            ref string __result,
            Item real_item)
        {
            // Add mod metadata to item description
            if (ModConfig.ShowExtraItemInfo)
            {
                var model = _repo.GetItem(__instance.id);
                if (model != null)
                {
                    string extraInfo = $"\n[Mod] Tier: {model.Quality}";
                    __result += extraInfo;
                }
            }
        }
    }
}
```

### 3.2 Craft Unlock Hook
**Location:** `SDK/Hooks/Crafting/CraftUnlockPatch.cs`
```csharp
using HarmonyLib;

namespace GraveSDK.Hooks.Crafting
{
    [HarmonyPatch(typeof(CraftDefinition))]
    [HarmonyPatch("IsLocked")]
    public static class CraftUnlockPatch
    {
        [HarmonyPostfix]
        public static void IsLocked_Postfix(
            CraftDefinition __instance,
            ref bool __result)
        {
            // Allow mods to override craft locks
            if (ModConfig.UnlockAllCrafts && __instance.needs_unlock)
            {
                __result = false;
            }
        }
    }
}
```

### 3.3 Localization Hook
**Location:** `SDK/Hooks/Localization/LocalizationPatch.cs`
```csharp
using HarmonyLib;

namespace GraveSDK.Hooks.Localization
{
    [HarmonyPatch(typeof(Localization))]
    [HarmonyPatch("Get")]
    public static class LocalizationPatch
    {
        [HarmonyPrefix]
        public static bool Get_Prefix(
            string key,
            bool warnIfMissing,
            ref string __result)
        {
            // Intercept localization for mod content
            if (key.StartsWith("mod_") && 
                CustomLocalization.TryGet(key, out string customText))
            {
                __result = customText;
                return false; // Skip original method
            }
            return true; // Continue with original
        }
    }
}
```

### 3.4 UI Localization Broadcast Hook
**Location:** `SDK/Hooks/UI/UIRootPatch.cs`
```csharp
using HarmonyLib;

namespace GraveSDK.Hooks.UI
{
    [HarmonyPatch(typeof(UIRoot))]
    [HarmonyPatch("Broadcast")]
    public static class UIRootPatch
    {
        [HarmonyPrefix]
        public static void Broadcast_Prefix(string funcName)
        {
            if (funcName == "OnLocalize")
            {
                // Notify mod UI of language change
                ModUIEvents.NotifyLanguageChanged();
            }
        }
    }
}
```

---

## 4. Mod Menu UI Components

### 4.1 Base Mod Menu
**Location:** `SDK/UI/ModMenu/ModMenuGUI.cs`
```csharp
using UnityEngine;

namespace GraveSDK.UI.ModMenu
{
    public class ModMenuGUI : MonoBehaviour
    {
        private bool _isVisible = false;
        private Vector2 _scrollPos;
        private TabType _currentTab = TabType.Items;
        
        private enum TabType
        {
            Items,
            Crafts,
            Buffs,
            Settings
        }
        
        void OnGUI()
        {
            if (!_isVisible) return;
            
            GUI.Window(0, new Rect(100, 100, 600, 400), DrawWindow, "Graveyard Keeper Mod Menu");
        }
        
        void DrawWindow(int windowID)
        {
            GUILayout.BeginVertical();
            
            // Tab selection
            GUILayout.BeginHorizontal();
            foreach (TabType tab in System.Enum.GetValues(typeof(TabType)))
            {
                if (GUILayout.Button(tab.ToString(), 
                    _currentTab == tab ? GUI.skin.button : GUI.skin.box))
                {
                    _currentTab = tab;
                }
            }
            GUILayout.EndHorizontal();
            
            // Tab content
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            {
                switch (_currentTab)
                {
                    case TabType.Items:
                        DrawItemsTab();
                        break;
                    case TabType.Crafts:
                        DrawCraftsTab();
                        break;
                    case TabType.Buffs:
                        DrawBuffsTab();
                        break;
                    case TabType.Settings:
                        DrawSettingsTab();
                        break;
                }
            }
            GUILayout.EndScrollView();
            
            GUILayout.EndVertical();
            
            GUI.DragWindow();
        }
        
        void DrawItemsTab()
        {
            GUILayout.Label("Item Browser", EditorStyles.boldLabel);
            // Item listing implementation
        }
        
        void DrawCraftsTab()
        {
            GUILayout.Label("Craft Browser", EditorStyles.boldLabel);
            // Craft listing implementation
        }
        
        void DrawBuffsTab()
        {
            GUILayout.Label("Buff Browser", EditorStyles.boldLabel);
            // Buff listing implementation
        }
        
        void DrawSettingsTab()
        {
            GUILayout.Label("Mod Settings", EditorStyles.boldLabel);
            ModConfig.UnlockAllCrafts = GUILayout.Toggle(
                ModConfig.UnlockAllCrafts, "Unlock All Crafts");
            ModConfig.ShowExtraItemInfo = GUILayout.Toggle(
                ModConfig.ShowExtraItemInfo, "Show Extra Item Info");
        }
        
        public void ToggleVisibility()
        {
            _isVisible = !_isVisible;
        }
    }
}
```

### 4.2 Item Browser Component
**Location:** `SDK/UI/Components/ItemBrowser.cs`
```csharp
using UnityEngine;

namespace GraveSDK.UI.Components
{
    public class ItemBrowser
    {
        private ItemRepository _repository;
        private List<ItemModel> _filteredItems;
        private string _searchFilter = "";
        
        public void Draw()
        {
            GUILayout.BeginVertical(GUI.skin.box);
            
            // Search bar
            _searchFilter = GUILayout.TextField(_searchFilter, 
                GUILayout.Width(200));
            
            if (GUILayout.Button("Refresh", GUILayout.Width(80)))
            {
                RefreshItems();
            }
            
            // Filter items
            _filteredItems = _repository.GetAllItems()
                .Where(i => string.IsNullOrEmpty(_searchFilter) || 
                    i.DisplayName.ToLower().Contains(_searchFilter.ToLower()) ||
                    i.Id.ToLower().Contains(_searchFilter.ToLower()))
                .ToList();
            
            // Item list
            foreach (var item in _filteredItems.Take(50)) // Limit display
            {
                DrawItemRow(item);
            }
            
            GUILayout.EndVertical();
        }
        
        void DrawItemRow(ItemModel item)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(item.Id, GUILayout.Width(150));
            GUILayout.Label(item.DisplayName, GUILayout.Width(200));
            GUILayout.Label($"Type: {item.Type}", GUILayout.Width(100));
            GUILayout.Label($"Q: {item.Quality}", GUILayout.Width(60));
            GUILayout.EndHorizontal();
        }
    }
}
```

---

## 5. Utility Extensions

### 5.1 GameRes Converter
**Location:** `SDK/Utils/Extensions/GameResExtensions.cs`
```csharp
namespace GraveSDK.Utils.Extensions
{
    public static class GameResExtensions
    {
        public static Dictionary<string, float> ToDictionary(
            this GameRes res)
        {
            // Convert GameRes to dictionary format
            // Implementation depends on GameRes structure
            return new Dictionary<string, float>();
        }
    }
}
```

### 5.2 Reflection Helpers
**Location:** `SDK/Utils/Extensions/ReflectionExtensions.cs`
```csharp
namespace GraveSDK.Utils.Extensions
{
    public static class ReflectionExtensions
    {
        public static T GetFieldValue<T>(
            this object obj, string fieldName)
        {
            var field = obj.GetType().GetField(
                fieldName, 
                BindingFlags.NonPublic | 
                BindingFlags.Public | 
                BindingFlags.Instance);
            
            return (T)field?.GetValue(obj);
        }
        
        public static void SetFieldValue<T>(
            this object obj, 
            string fieldName, 
            T value)
        {
            var field = obj.GetType().GetField(
                fieldName,
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.Instance);
            
            field?.SetValue(obj, value);
        }
    }
}
```

---

## 6. SDK Initialization

### 6.1 Mod Entry Point
**Location:** `SDK/SDKEntry.cs`
```csharp
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace GraveSDK
{
    [BepInPlugin(
        "com.graveyardkeeper.sdk",
        "Graveyard Keeper SDK",
        "1.0.0")]
    public class SDKEntry : BaseUnityPlugin
    {
        private static Harmony _harmony;
        private static ModMenuGUI _modMenu;
        
        void Awake()
        {
            Logger.LogInfo("Graveyard Keeper SDK loaded!");
            
            // Initialize Harmony patches
            _harmony = new Harmony("com.graveyardkeeper.sdk");
            _harmony.PatchAll();
            Logger.LogInfo("Harmony patches applied");
            
            // Setup mod menu hotkey
            ModConfig.SetupHotkey();
        }
        
        void Update()
        {
            // Toggle mod menu
            if (ModConfig.IsHotkeyPressed())
            {
                if (_modMenu == null)
                {
                    var go = new GameObject("SDK_ModMenu");
                    _modMenu = go.AddComponent<ModMenuGUI>();
                }
                _modMenu.ToggleVisibility();
            }
        }
        
        void OnDestroy()
        {
            _harmony?.UnpatchAll();
            Logger.LogInfo("Graveyard Keeper SDK unloaded");
        }
    }
}
```

### 6.2 Mod Configuration
**Location:** `SDK/Utils/ModConfig.cs`
```csharp
namespace GraveSDK.Utils
{
    public static class ModConfig
    {
        public static bool UnlockAllCrafts { get; set; } = false;
        public static bool ShowExtraItemInfo { get; set; } = true;
        public static KeyCode MenuHotkey { get; set; } = KeyCode.F1;
        
        public static void SetupHotkey()
        {
            // Load from config file
            // BepInEx config system integration
        }
        
        public static bool IsHotkeyPressed()
        {
            return Input.GetKeyDown(MenuHotkey);
        }
    }
}
```

---

## 7. Implementation Roadmap

### Phase 1: Foundation (COMPLETED)
- ✅ Repository structure created
- ✅ Documentation framework established
- ✅ Data models defined
- ✅ Basic wrapper classes outlined

### Phase 2: Core Wrappers
- [x] ItemRepository - Full implementation
- [x] CraftRepository - Full implementation  
- [x] BuffRepository - Full implementation
- [x] PerkRepository - Full implementation
- [x] VendorRepository - Full implementation
- [x] ObjectRepository - Full implementation
- [x] QuestRepository - Full implementation
- [x] TechRepository - Full implementation
- [x] NPCRepository - Full implementation
- [x] UIRepository - Full implementation
- [x] WorldRepository - Full implementation
- [x] InventoryRepository - Full implementation
- [x] SaveRepository - Full implementation
- [x] PlayerRepository - Full implementation
- [x] EnvironmentRepository - Full implementation
- [x] LocalizationRepository - Full implementation

### Phase 3: Harmony Hooks
- [x] Item description patches
- [x] Craft unlock patches
- [x] Localization patches
- [x] UI event patches
- [x] MainGame engine hook (`Hooks/Engine/MainGamePatch.cs`)
- [x] Movement speed hook (`Hooks/Engine/MovementPatch.cs`)

### Phase 4: Mod Menu UI
- [x] Main menu GUI (IMGUI Mod Menu)
- [x] Item browser
- [x] Craft browser
- [x] Settings panel (Placeholder)

### Phase 5: Advanced Features
- [x] Save/load mod config
- [x] Custom recipe injection (RegistryRepository)
- [x] Mod loader integration (ModLoader)
- [x] Item spawn commands (Mod Menu)
- [x] Game state manipulation (Mod Menu)

---

## 8. Dependencies & Requirements

### Required
- **BepInEx** - Plugin framework
- **HarmonyX** - Runtime patching
- **Unity** - Game engine (matching version)

### Optional
- **UnityEngine.UI** - Enhanced UI components
- **Newtonsoft.Json** - Config serialization

---

## 9. Safety & Best Practices

### Do's ✅
- Use wrapper classes for all game object access
- Separate data, logic, and UI concerns
- Implement graceful fallbacks for missing data
- Test patches in isolation

### Don'ts ❌
- ❌ Modify original game DLL files
- ❌ Access private fields without reflection
- ❌ Block or remove original game functionality
- ❌ Create memory leaks with event subscriptions
- ❌ Use magic strings - use constants/enums

---

*SDK Architecture Documentation*
*Generated: 2026-05-04*

---

## 10. Build Fixes & API Alignments (2026-05-04)

During the initial build process, several mismatches between the proposed architecture and the actual game code were identified and fixed.

### 10.1 GameBalance Field Names
The game uses `items_data` instead of `item_data` in the `GameBalance` class. All repository code has been updated to reflect this.

### 10.2 SmartExpression API
The `SmartExpression` class in the game does not allow direct property assignment for string expressions. It must be initialized using `FromString()`:
```csharp
var expr = new SmartExpression();
expr.FromString("expression_string");
```
And the raw string can be retrieved using `GetRawExpressionString()`.

### 10.3 GameState & Flags
The global `GameState` class was not present in the managed assemblies. Instead, game flags and parameters are accessed via the player object:
```csharp
bool hasFlag = MainGame.me.player.GetParamInt(flagId) > 0;
```

### 10.4 Assembly Dependencies
To successfully build the SDK, the following assemblies must be referenced:
- `Assembly-CSharp-firstpass.dll`: Contains NGUI and core utility classes (`SmartExpression`, `NGUIText`).
- `UnityEngine.InputLegacyModule.dll`: Required for `UnityEngine.Input` access in Unity 2019+ (Grave Keeper uses this module).
- `UnityEngine.IMGUIModule.dll`: Required for the Mod Menu UI.

### 10.5 Enum Casting
Since the SDK defines its own type-safe enums (`ItemType`, `CraftType`), explicit casts are required when mapping from the native game enums. For example:
```csharp
var sdkType = (GraveSDK.Data.Models.CraftType)nativeCraft.craft_type;
```
This ensures that the SDK remains decoupled from specific game assembly versions where possible.

### 10.6 Player Money & Data
Player money is not stored directly in `GameSave`. Instead, it is a property on the `Item` object stored in `WorldGameObject.data`. 
- **Access**: `MainGame.me.player.data.money`
- **Type**: `float` (representing bronze as the decimal part)

### 10.7 MultiInventory & GetMultiInventory
The `WorldGameObject.GetMultiInventory` method requires specific arguments for world zones and player exclusion:
```csharp
// To get container-only inventory:
container.GetMultiInventory(null, "", MultiInventory.PlayerMultiInventory.ExcludePlayer, true, false, false);
```

### 10.8 GetParamInt Overload
The `GetParamInt(string)` method does NOT take a default value. To use a default value, use `(int)GetParam(string, float)` instead.