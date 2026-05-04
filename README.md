<p align="center">
  <h1 align="center">⚰️ GraveYardKeeper SDK</h1>
  <p align="center">
    <strong>A comprehensive modding framework for Graveyard Keeper</strong>
  </p>
  <p align="center">
    <code>v1.3 · Reactive Framework</code>
  </p>
</p>

---

A modular, event-driven SDK for creating Graveyard Keeper mods. Built on **BepInEx** and **HarmonyX**, the SDK wraps the game's internal systems into clean repository classes — giving modders safe, documented access to items, NPCs, quests, dialogue, cameras, workers, vendors, and more.

## Features at a Glance

| System | What You Can Do |
|---|---|
| **Items & Inventory** | Query, add, remove, and modify items and crafting recipes |
| **NPCs & Dialogue** | Make NPCs talk, show branching choices, change skins, teleport |
| **Quests** | Create custom quests with SmartExpression triggers and rewards |
| **Camera** | Screen shake, fades, letterboxing, 100+ cinematic filters |
| **Workers** | Adjust zombie efficiency, pick up/deploy workers |
| **Vendors & Economy** | Create new merchants, set prices, manipulate trade inventories |
| **Game Events** | Subscribe to day changes, crafting, saves, deaths, interactions |
| **Custom Interactions** | Add new interaction buttons to any world object |
| **Player** | Modify stats, money, tech points, teleport |
| **Environment** | Control time of day, weather |

---

## Quick Start

### Prerequisites

- [BepInEx 5.x](https://github.com/BepInEx/BepInEx) installed in your Graveyard Keeper directory
- [HarmonyX](https://github.com/BepInEx/HarmonyX) (included with BepInEx)
- .NET Standard 2.0 compatible build environment

### Build

```bash
dotnet build SDK/SDK.csproj
```

### Deploy

Copy the compiled DLL to your BepInEx plugins folder:

```
Graveyard Keeper/BepInEx/plugins/GraveYardKeeperSDK.dll
```

### Create a Mod

Use the included template as a starting point:

```bash
dotnet build Template/GraveYardKeeperModTemplate/GraveYardKeeperModTemplate.csproj
```

See [`Template/GraveYardKeeperModTemplate/ModEntry.cs`](Template/GraveYardKeeperModTemplate/ModEntry.cs) for a full working example with hotkey demos.

---

## SDK Architecture

The SDK follows strict **Separation of Concerns** — all game access goes through repository classes, never touching game code directly.

```
SDK/
├── Core/                          # Framework bootstrap
│   ├── Plugin.cs                  # BepInEx entry point
│   ├── ModLoader.cs               # Mod discovery & loading
│   ├── ModConfig.cs               # Persistent config system
│   └── Interfaces.cs              # Mod interface contracts
│
├── Data/
│   ├── Models/                    # 12 type-safe data models
│   │   ├── ItemModel.cs
│   │   ├── CraftModel.cs
│   │   ├── QuestModel.cs
│   │   └── ...
│   │
│   └── Repositories/              # 24 data access repositories
│       ├── ItemRepository.cs      # Item queries & injection
│       ├── CraftRepository.cs     # Crafting recipe access
│       ├── PlayerRepository.cs    # Player stats & teleport
│       ├── NPCRepository.cs       # NPC skins & relationships
│       ├── QuestRepository.cs     # Quest state management
│       ├── DialogueRepository.cs  # Speech bubbles & choices
│       ├── CameraRepository.cs    # Cinematic effects
│       ├── WorkerRepository.cs    # Zombie management
│       ├── VendorRepository.cs    # Trade list manipulation
│       ├── EconomyRepository.cs   # Full vendor creation & pricing
│       ├── InteractionRepository.cs # Custom WGO interactions
│       ├── GameEventRepository.cs # Reactive event bus
│       ├── SmartExpressionRepository.cs # Expression engine & QuestBuilder
│       └── ...
│
├── Hooks/                         # Harmony patches
│   ├── Events/                    # Game lifecycle patches
│   ├── Inventory/                 # Item tooltip hooks
│   ├── Crafting/                  # Craft system hooks
│   └── ...
│
├── UI/
│   └── ModMenu/                   # In-game mod menu (F1)
│
└── Utils/                         # Config, extensions, helpers
```

---

## Code Examples

### Modify Player Stats

```csharp
var player = new PlayerRepository();
player.AddMoney(100000);           // Add 10 gold
player.AddTechPoints(50, 50, 50);  // Red, Green, Blue
player.SetEnergy(100f);
player.TeleportTo(new Vector3(0, 0, 0));
```

### Make an NPC Talk

```csharp
var world = new WorldRepository();
var gerry = world.GetNPC("gerry");

// Speech bubble
DialogueRepository.Say(gerry, "Hello, friend!", SpeechBubbleGUI.SpeechBubbleType.Talk);

// Branching choices
DialogueRepository.ShowOptions(gerry, new List<string> {
    "Tell me about the graveyard",
    "What's for sale?",
    "Goodbye"
}, (choice) => {
    DialogueRepository.SayAsPlayer("I chose: " + choice);
});

// Corner notification
DialogueRepository.ShowCornerMessage("Something feels different...");
```

### Create a Custom Quest with Triggers

```csharp
// Quest: "Kill 5 slimes before day 20"
var quest = new QuestBuilder("mod_slay_slimes")
    .SetStartKey("interact_gerry")                      // Triggers when talking to Gerry
    .SetSuccessTrigger("Ppar(\"slimes_killed\") >= 5")   // Completes at 5 kills
    .SetFailTrigger("GetDay() > 20")                     // Fails after day 20
    .SetVisible(true)
    .SetOneTime(true)
    .AddSuccessExpression("AddPpar(\"money\", 5000)")     // Reward: 50 gold
    .BuildAndRegister();
```

### React to Game Events

```csharp
// No polling needed — subscribe once
GameEventRepository.OnDayChanged += (day) => {
    Debug.Log("New day: " + day);
};

GameEventRepository.OnCraftCompleted += (station, craft) => {
    Debug.Log("Crafted: " + craft.id);
};

GameEventRepository.OnPlayerInteract += (target) => {
    Debug.Log("Interacted with: " + target.obj_id);
};
```

### Add Custom Interactions to Objects

```csharp
var world = new WorldRepository();
var well = world.FindByTag("well");

// Add a "Wish" interaction
InteractionRepository.AddInteraction(well, "make_wish");
InteractionRepository.RegisterCallback("make_wish", (wgo) => {
    DialogueRepository.ShowCornerMessage("You toss a coin and make a wish...");
    player.AddMoney(10000);  // Wishes come true!
});
```

### Create a New Merchant

```csharp
EconomyRepository.CreateVendorDefinition(
    vendorId: "mod_wandering_trader",
    startMoney: 500f,
    startTier: 2,
    dailyIncome: 100f,
    productTypes: new List<string> { "weapon", "armor", "potion" }
);
```

### Cinematic Camera Effects

```csharp
CameraRepository.Shake(intensity: 3f, duration: 0.5f);
CameraRepository.Fade(true, duration: 1f);       // Fade to black
CameraRepository.SetFilter("CameraFilterPack_TV_Old", true);
CameraRepository.FocusOn(gerry, duration: 2f);
```

---

## SmartExpression Reference

The SDK exposes the game's built-in expression engine. These expressions power quest triggers, interaction conditions, and scripted logic.

| Function | Description | Example |
|---|---|---|
| `Ppar("name")` | Get player parameter | `Ppar("money")` |
| `SetPpar("name", val)` | Set player parameter | `SetPpar("hp", 100)` |
| `AddPpar("name", val)` | Add to player parameter | `AddPpar("money", 5000)` |
| `WGOpar("name")` | Get WGO parameter | `WGOpar("quality")` |
| `GetDay()` | Current game day | `GetDay() > 10` |
| `IsDay()` / `IsNight()` | Time of day (returns 0/1) | `IsNight()` |
| `GetTime()` | Time as 0-1 float | `GetTime() > 0.5` |
| `HasItemInWGO("id")` | Check WGO inventory | `HasItemInWGO("wood")` |
| `HasOverheadBody()` | Player carrying a body? | `HasOverheadBody()` |

**Shorthand syntax** (auto-parsed by the game):
- `$money` → `Ppar("money")`
- `$money += 100` → `AddPpar("money", 100)`
- `@quality` → `WGOpar("quality")`

---

## Game Event Bus

All events are fired automatically via Harmony patches. Subscribe in your mod's `Awake()` or `Start()`.

| Event | Signature | Fires When |
|---|---|---|
| `OnDayChanged` | `Action<int>` | New day begins |
| `OnPlayerInteract` | `Action<WorldGameObject>` | Player interacts with any WGO |
| `OnCraftCompleted` | `Action<WorldGameObject, CraftDefinition>` | Any craft finishes |
| `OnGameSaved` | `Action` | Game is saved |
| `OnGameLoaded` | `Action` | Save file loaded |
| `OnPlayerDied` | `Action` | Player dies |
| `OnGameTick` | `Action<float>` | Every frame (when unpaused) |
| `OnNPCArrived` | `Action<WorldGameObject>` | NPC spawns/arrives |
| `OnBodyDelivered` | `Action` | Donkey delivers a body |

---

## Repository Reference

| Repository | Type | Key Methods |
|---|---|---|
| `ItemRepository` | Instance | `GetItem`, `GetItemsByType`, `GetAllItems`, `SearchByName` |
| `CraftRepository` | Instance | `GetCraft`, `GetCraftsForStation`, `GetAllCrafts` |
| `InventoryRepository` | Instance | `AddItem`, `RemoveItem`, `HasItem`, `GetItemCount` |
| `PlayerRepository` | Instance | `GetStats`, `SetEnergy`, `AddMoney`, `AddTechPoints`, `TeleportTo` |
| `WorldRepository` | Instance | `SpawnObject`, `FindByTag`, `GetNPC`, `GetAllNPCs`, `GetNearestObject` |
| `NPCRepository` | Instance | `SetSkin`, `TeleportToPlayer`, `AddToKnownNPCs` |
| `QuestRepository` | Instance | `StartQuest`, `ForceComplete`, `ForceFail`, `CheckKey`, `GetCurrentQuests` |
| `VendorRepository` | Instance | `AddItemToTrade`, `SetMoney` |
| `DialogueRepository` | Static | `Say`, `SayAsPlayer`, `ShowOptions`, `ShowCornerMessage` |
| `CameraRepository` | Static | `Shake`, `Fade`, `SetFilter`, `FocusOn` |
| `WorkerRepository` | Static | `SetEfficiency`, `PickUpWorker`, `DeployWorker` |
| `InteractionRepository` | Static | `AddInteraction`, `RemoveInteraction`, `FireEvent`, `RegisterCallback` |
| `GameEventRepository` | Static | `OnDayChanged`, `OnCraftCompleted`, `OnPlayerInteract`, + 6 more |
| `EconomyRepository` | Static | `CreateVendorDefinition`, `SetVendorMoney`, `SetItemBasePrice` |
| `SmartExpressionRepository` | Static | `Create`, `EvaluateFloat`, `EvaluateBoolean`, `Execute` |
| `QuestBuilder` | Builder | `SetStartTrigger`, `SetSuccessTrigger`, `BuildAndRegister` |

---

## Project Structure

```
GraveYardKeeper/
├── SDK/                           # The SDK framework (GraveYardKeeperSDK.dll)
├── Template/                      # Example mod project (ExampleMod.dll)
├── GameSource/                    # Decompiled game source (reference only)
├── docs/
│   ├── Wiki_Findings.md           # Game data documentation
│   └── Code_Changes.md            # SDK architecture & implementation guide
└── README.md
```

## Safety Principles

- ✅ **Never modify game DLLs** — all access through wrapper classes
- ✅ **Null-safe everywhere** — graceful fallbacks for missing data
- ✅ **Event cleanup** — `ClearAll()` methods on all event buses
- ✅ **Cached access** — repositories cache data to avoid repeated lookups
- ✅ **Harmony patches only** — runtime patching, no file modification

## Contributing

1. Fork the repository
2. Create a feature branch
3. Submit a pull request

## License

This SDK is provided as-is for educational and modding purposes. All game assets and original code belong to Lazy Bear Games.