# Graveyard Keeper Modding SDK

## Overview
This repository contains a comprehensive Modding SDK and documentation for *Graveyard Keeper*, built from decompiled source code analysis. The SDK provides a modular foundation for creating mods, mod menus, and custom content without modifying original game files.

## Repository Structure
```
graveyard-keeper-sdk/
├── docs/
│   ├── Wiki_Findings.md       # Extracted game data (items, crafts, buffs, etc.)
│   └── Code_Changes.md        # SDK architecture and implementation guide
├── SDK/
│   ├── Hooks/                 # Harmony patch classes
│   │   ├── Inventory/         # Item-related hooks
│   │   ├── Crafting/          # Craft-related hooks
│   │   ├── UI/                # UI-related hooks
│   │   ├── Localization/      # Localization hooks
│   │   └── Data/              # Data access hooks
│   ├── Data/
│   │   ├── Models/            # Data models (Item, Craft, Buff)
│   │   └── Repositories/      # Data access repositories
│   ├── UI/
│   │   ├── Components/        # Reusable UI components
│   │   └── ModMenu/           # Main mod menu implementation
│   └── Utils/
│       └── Extensions/        # Helper extensions
├── Template/                  # Mod Template Project
│   └── GraveYardKeeperModTemplate/
└── README.md
```

## Quick Start

### 1. Install Dependencies
- [BepInEx](https://github.com/BepInEx/BepInEx) - Plugin framework
- [HarmonyX](https://github.com/BepInEx/HarmonyX) - Runtime patching

### 2. Build the SDK
```bash
# Reference BepInEx and HarmonyX assemblies
# Build with .NET Framework 4.x (matching Unity version)
```

### 3. Deploy to Game
Place compiled DLL in:
```
Graveyard Keeper\BepInEx\plugins\
```

### 4. Use Mod Menu
Press **F1** in-game to toggle mod menu.

### 5. Create Your Own Mod
Check the [Template](Template/GraveYardKeeperModTemplate/) folder for a comprehensive example mod demonstrating all SDK features.

## Documentation Index

### docs/Wiki_Findings.md - Extracted Game Data (488 lines)
**Sections:**
1. **Item System** - ItemDefinition, 30+ item types, quality systems, equipment types, bag types
2. **Crafting System** - CraftDefinition, 8 craft types, 3 sub-types, enqueue types
3. **Buff System** - BuffDefinition, 2 overlay types, status effects
4. **Localization System** - Localization class, multi-language support (637 tokens)
5. **UI Components** - 100+ NGUI widget classes (UIPanel, UIButton, UILabel, etc.)
6. **Core Data Structures** - GameBalance system with all registries, MainGame lifecycle
   - GameBalance: 37 data lists (items, crafts, objects, techs, quests, etc.)
   - MainGame: GeneralInit, InGameUpdate, OnPlayerDied methods
7. **Hook Points Summary** - 5 high-priority Harmony patch targets

**Key Classes Documented:**
- ItemDefinition (876 tokens)
- CraftDefinition (860 tokens)
- BuffDefinition (828 tokens)
- Localization (637 tokens)
- ObjectDefinition, QuestDefinition, TechDefinition, PerkDefinition
- FishDefinition, EventDefinition, ObjectInteractionDefinition

**Enums Documented:** 20+ types (ItemType, CraftType, QualityType, AlchemyType, etc.)

---

### docs/Code_Changes.md - SDK Architecture (948 lines)
**Sections:**
1. **Data Models (SoC Layer 1)** - ItemModel, CraftModel, BuffModel with all enums
2. **Game Data Registries** - GameBalance system, all data lists, loading process
3. **Key Functions & Hook Points** - MainGame methods with line numbers
4. **SDK Architecture & SoC Integration** - Proposed hooks and DataRegistry pattern
5. **Wrapper Classes (SoC Layer 2)** - ItemRepository, CraftRepository with caching
6. **Harmony Hook Patches** - 7 patch examples (Item, Craft, Localization hooks)
7. **Mod Menu UI Components** - ModMenuGUI, ItemBrowser implementations
8. **Utility Extensions** - ModConfig, reflection helpers, interfaces
9. **SDK Initialization** - SDKEntry, BepInEx plugin setup
10. **Implementation Roadmap** - 5 phases with task tracking
11. **Dependencies & Requirements** - BepInEx, HarmonyX, Unity
12. **Safety & Best Practices** - Do's and Don'ts for modding

**Code Examples:** 12+ complete classes with 500+ lines of example code

---

### SDK/ - Implementation Files (6 classes)
- **SDK/Data/Models/ItemModel.cs** - Type-safe data models (222 lines)
- **SDK/Data/Repositories/ItemRepository.cs** - Item data access with caching
- **SDK/Data/Repositories/CraftRepository.cs** - Craft data access
- **SDK/Hooks/Inventory/ItemDescriptionPatch.cs** - Item tooltip hooks
- **SDK/Hooks/Crafting/CraftDefinitionPatch.cs** - Craft system hooks
- **SDK/Utils/ModConfig.cs** - Configuration system with interfaces

---

## Quick Stats
- **Total Documentation:** 1,436+ lines across 5 files
- **Source Files Analyzed:** 200+ game files
- **Classes Documented:** 100+
- **Enum Types:** 20+
- **Example Code Lines:** 500+
- **Harmony Patch Examples:** 7
- **Data Repository Classes:** 3

## Key Features

### Data Access
```csharp
var itemRepo = new ItemRepository();
var items = itemRepo.GetItemsByType(ItemType.Weapon);

var craftRepo = new CraftRepository();
var craft = craftRepo.GetCraft("craft:sword_iron");
```

### Modding Hooks
```csharp
[HarmonyPatch(typeof(ItemDefinition))]
[HarmonyPatch("GetItemDescription")]
public static class ItemHook
{
    [HarmonyPostfix]
    public static void AddModInfo(ref string __result)
    {
        __result += "\n[Mod] Enhanced by SDK!";
    }
}
```

### Custom UI
```csharp
public class MyModMenu : MonoBehaviour
{
    void OnGUI()
    {
        GUILayout.Window(0, rect, DrawWindow, "My Mod");
    }
}
```

## Architecture Principles

### Separation of Concerns (SoC)
1. **Data Layer** - Models and repositories
2. **Logic Layer** - Harmony patches and hooks
3. **Presentation Layer** - UI components
4. **Utility Layer** - Extensions and helpers

### Safety First
- ✅ Never modify original game DLLs
- ✅ Use wrapper classes for all access
- ✅ Graceful fallbacks for missing data
- ✅ Proper event cleanup
- ✅ Type-safe operations

## Game Systems Documented

### Core Systems
- **Item System** - 100+ item types, quality tiers, durability
- **Crafting System** - 8 craft types, multi-quality, auto-craft
- **Buff System** - Status effects, timers, resource modifiers
- **Localization** - CSV-based multi-language support
- **UI Framework** - NGUI widget library

### Specialized Systems
- **Body Parts** - Segmented character system
- **Alchemy** - 3-tier powder/fluid/essence system
- **Survey** - Science point generation
- **Rat System** - Pet mechanics
- **Sermons** - Prayer/craft mechanics

## Statistics

- **Source Files Analyzed**: 200+
- **Classes Documented**: 100+
- **Enum Types**: 20+
- **Total Lines Analyzed**: 10,000+
- **Key Classes**: ItemDefinition, CraftDefinition, BuffDefinition, Localization

## Contributing

1. Fork the repository
2. Create a feature branch
3. Add documentation and/or code
4. Submit a pull request

## License

This SDK is provided as-is for educational and modding purposes. All game assets and original code belong to their respective owners.

## Contact

For issues or questions, please open a GitHub issue.

--- 

*Generated from decompiled source code analysis*
*Source: Graveyard Keeper v1.0+*
*SDK Version: v1.2 (Feature Expansion)*

## v1.2 Highlights:
- **Dialogue System**: Full control over NPC speech, player choices, and cinematic messages.
- **Camera Effects**: Screen shake, fades, and toggling 100+ built-in cinematic filters.
- **Worker Management**: Zombie efficiency control and worker item/WGO conversion.
- **Enhanced Data Repositories**: Programmatic Quest, Vendor, and NPC skin management.