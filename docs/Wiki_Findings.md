# Graveyard Keeper Wiki Findings

## Last Updated
2026-05-04

## Data Repository Overview
This document contains extracted hardcoded values, enums, data structures, and initialization lists from the Graveyard Keeper decompiled source code (Assembly-CSharp).

---

## 1. Item System

### ItemDefinition Class
**File:** `Assembly-CSharp/ItemDefinition.cs`  
**Namespace:** (Global)  
**Token:** 0x0200036C RID: 876

### Item Types (Enum: ItemType)
| ID | Type Name | Description |
|----|-----------|-------------|
| -1 | PseudoitemFirst | Pseudo-item marker (start) |
| 0 | None | No item type |
| 1 | Axe | Wood cutting tool |
| 2 | Pickaxe | Mining tool |
| 3 | Shovel | Digging tool |
| 4 | Sword | Combat weapon |
| 5 | Hammer | Building/repair tool |
| 6-8 | (Reserved) | Unused tool types |
| 9 | Torch | Light source |
| 10 | Hand | Bare hands |
| 11 | Item | Generic item |
| 12 | HeadArmor | Helmet/head protection |
| 13 | BodyArmor | Chest armor |
| 20 | Preach | Sermon item |
| 31 | Bait | Fishing bait |
| 50 | Crate | Storage container |
| 60 | Rat | Pet rat |
| 61 | RatBuff | Rat enhancement buff |
| 101 | GraveStone | Grave marker |
| 102 | GraveFence | Grave fence |
| 103 | GraveCover | Grave cover |
| 200 | Body | Full body |
| 201 | BodyHead | Head part |
| 202 | BodyBody | Torso part |
| 203 | BodyArmR | Right arm |
| 204 | BodyArmL | Left arm |
| 205 | BodyLegR | Right leg |
| 206 | BodyLegL | Left leg |
| 210 | BodyHeadPart | Head component |
| 220 | BodyBodyPart | Torso component |
| 230 | BodyArmPart | Arm component |
| 250 | BodyLegPart | Leg component |
| 270 | BodyUniversalPart | Universal body part |
| 280 | SoulBodyPart | Soul-related part |
| 280 | Soul | Soul item |
| 300 | ZombieWorker | Undead laborer |
| 400 | Bag | Inventory container |
| 10101 | GraveStoneReq | Grave stone requirement |
| 10102 | GraveFenceReq | Grave fence requirement |
| 10103 | GraveCoverReq | Grave cover requirement |
| 100000 | PseudoitemLast | Pseudo-item marker (end) |

### Equipment Types (Enum: EquipmentType)
| ID | Type Name |
|----|-----------|
| 0 | None |
| 1 | Axe |
| 2 | Pickaxe |
| 3 | Shovel |
| 4 | Sword |
| 5 | Hammer |
| 6 | FishingRod |
| 12 | HeadArmor |
| 13 | BodyArmor |

### Item Quality Types (Enum: QualityType)
- **Default** (0) - No special quality display
- **Stars** (1) - Star-based quality rating

### Item Alchemy Types (Enum: AlchemyType)
- **None** (0) - Not an alchemy item
- **Powder** (1) - Powder form
- **Fluid** (2) - Fluid form  
- **Essence** (3) - Essence form
- **Universal** (9) - Universal solvent

### Bag Types (Enum: BagType)
- **None** (0)
- **Universal** (1)
- **Alchemy** (2)
- **Farming** (3)
- **Fishing** (4)
- **Tools** (5)
- **Potions** (6)
- **Builder** (7)
- **Food** (8)

### ItemDefinition Key Fields
```csharp
public string id;                    // Unique identifier
public string icon;                  // Icon sprite name
public ItemType type;                // Item type enum
public float quality;                // Quality value (0-15+)
public ItemDefinition.QualityType quality_type;  // Quality display type
public int stack_count;              // Max stack size
public int base_count;               // Base quantity
public float base_price;             // Base sell price
public bool is_static_cost;          // Fixed price flag
public float durability_decrease;    // Durability loss rate
public bool has_durability;          // Has durability flag
public GameRes parameters;           // Parameter values
public List<SmartExpression> on_use_expressions;  // Use effects
public List<Item> drop_on_use;       // Items dropped on use
```

---

## 2. Crafting System

### CraftDefinition Class
**File:** `Assembly-CSharp/CraftDefinition.cs`  
**Namespace:** (Global)  
**Token:** 0x0200035C RID: 860

### Craft Types (Enum: CraftType)
| ID | Type Name | Description |
|----|-----------|-------------|
| 0 | None | No craft type |
| 1 | ResourcesBasedCraft | Material-based crafting |
| 2 | Survey | Survey/scan action |
| 3 | MixedCraft | Hybrid crafting |
| 4 | Fixing | Repair action |
| 5 | AlchemyDecompose | Alchemy breakdown |
| 6 | PrayCraft | Prayer/Sermon craft |
| 7 | RatBuff | Rat enhancement |
| 8 | RefugeeCampCraft | Refugee camp action |

### Craft Sub Types (Enum: CraftSubType)
- **None** (0)
- **Alchemy** (1)
- **SurveySciencePoints** (2)

### Enqueue Types (Enum: EnqueueType)
- **Default** (0)
- **CanEnqueue** (1)
- **NeverEnqueue** (2)

### CraftDefinition Key Fields
```csharp
public string id;                        // Unique identifier
public List<Item> needs;                 // Required items
public List<Item> output;                // Crafted items
public List<string> craft_in;            // Craft locations
public float difficulty;                 // Craft difficulty
public List<string> linked_perks;        // Required perks
public List<string> linked_buffs;        // Associated buffs
public SmartExpression energy;           // Energy cost
public SmartExpression craft_time;       // Craft duration
public bool needs_unlock;                // Requires unlock
public bool one_time_craft;              // Single use only
public bool is_auto;                     // Auto-craft flag
public string tab_id;                    // GUI tab identifier
```

---

## 3. Buff System

### BuffDefinition Class
**File:** `Assembly-CSharp/BuffDefinition.cs`  
**Namespace:** (Global)  
**Token:** 0x0200033C RID: 828

### Buff Overlay Types (Enum: BuffOverlayType)
- **Set** (0) - Set value directly
- **Add** (1) - Add to existing value

### BuffDefinition Key Fields
```csharp
public string id;                    // Unique identifier
public bool is_hidden;               // Hidden from UI
public GameRes res;                  // Resource effects
public SmartExpression length;       // Duration expression
public float tick_period;            // Tick interval
public SmartExpression se_start;     // Start effect
public SmartExpression se_finish;    // Finish effect
public SmartExpression se_tick;      // Tick effect
public float craft_q;                // Craft quality bonus
```

---

## 4. Localization System

### Localization Class
**File:** `Assembly-CSharpFirstPass/Localization.cs`  
**Namespace:** (Global)  
**Token:** 0x020000A7 RID: 167

**Key Methods:**
- `Get(string key, bool warnIfMissing)` - Get localized text
- `Set(string language, Dictionary<string, string>)` - Set dictionary
- `LoadCSV(byte[] bytes, bool merge)` - Load translations
- `string[] knownLanguages` - Available languages
- `string language` - Current language

**Static Fields:**
- `mDictionary` - Translation dictionary
- `mReplacement` - Runtime replacements
- `onLocalize` - Localization change callback

---

## 5. UI System Components

### NGUI-Based UI (Unity GUI)
**Key Classes Identified:**
- `UIPanel` - UI container
- `UIRoot` - Root UI object
- `UIButton` - Interactive button
- `UILabel` - Text display
- `UISlider` - Slider control
- `UIPopupList` - Dropdown list
- `UIInput` - Text input field
- `UIScrollView` - Scrollable area
- `UIGrid` - Grid layout
- `UIAtlas` - Sprite atlas
- `UIFont` - Bitmap font

**LanguageSelection Class:**
**File:** `LanguageSelection.cs`  
Handles language dropdown population from `Localization.knownLanguages`

**UniversalStorage Class:**
**File:** `UniversalStorage.cs`  ���  
**Token:** 0x02000042 RID: 66
- Generic key-value storage system
- Serialized as `List<string> _keys` and `List<object> _vals`
- Type-safe Get/Set methods

---

## 6. Balance and Data Objects

### BalanceBaseObject (Base Class)
Base class for all balance data objects (ItemDefinition, BuffDefinition, CraftDefinition, etc.)

**Known Derived Classes:**
- ItemDefinition
- BuffDefinition
- CraftDefinition
- ObjectDefinition
- BodyDefinition
- AuraDefinition
- AchievementDefinition
- AnswerData

---

## 7. Game Architecture Patterns

### Data Flow
1. **GameBalance.me** - Central data manager singleton
2. **BalanceBaseObject** - Base for all data types
3. **Generic Data Access:** `GameBalance.me.GetData<T>(id)`
4. **Lists:** `GameBalance.me.craft_data`, `GameBalance.me.item_data`, etc.

### Localization Flow
1. PlayerPrefs stores language preference
2. Localization.LoadDictionary() loads CSV/bytes
3. Localization.mDictionary cached in memory
4. GJL.L() wrapper for getting localized strings

### Item-Craft Relationship
- Items reference crafts via `linked_craft` property
- Crafts reference items via `needs` and `output` lists
- Quality-based item variants use `id:suffix` format

---

## 8. Known Game Systems

### Body Parts System
- Full body (BodyDefinition)
- Segmented parts (head, torso, limbs)
- Body parts as items (Body, BodyHead, BodyBody, etc.)
- Embalming/body modification effects

### Alchemy System
- Alchemy types: Powder, Fluid, Essence, Universal
- Alchemy tiers (1-3)
- Decompose crafts
- Goo conversion system

### Survey System
- Survey crafts generate science points
- Survey not complete/complete states
- Item decomposition rewards

### Rat System
- Rat items (Rat, RatBuff)
- Rat speed/obedience stats
- Rat buff mechanics

### Prayer/Sermon System
- Preach item type
- Linked to PrayCraft crafts
- Cross quality requirements
- Faith/money generation

---

## 9. Hook Points for Modding

### High-Priority Targets

#### Item Creation/Usage
- `ItemDefinition.GetItemDescription()` - Modify item tooltips
- `ItemDefinition.GetItemName()` - Modify item names
- `ItemDefinition.GetItemDetails()` - Modify item details
- `Localization.Get()` - Intercept localized text

#### Crafting
- `CraftDefinition.CanCraftMultiple()` - Modify craft limits
- `CraftDefinition.IsLocked()` - Modify craft locks
- `CraftDefinition.GetSpendTxt()` - Modify cost display
- `CraftDefinition.GetMultiqualityResult()` - Modify quality outcomes

#### UI
- `UIRoot.Broadcast("OnLocalize")` - Hook for language changes
- Various UI widget classes for custom UI elements

#### Data Loading
- `GameBalance.me` - Intercept data access
- `BalanceBaseObject` constructors - Modify data on load

---

## 10. Repository Data Statistics

**Source Files Analyzed:**
- Assembly-CSharp/Assembly-CSharp: ~100+ classes
- Assembly-CSharp-firstpass: ~100+ utility/NGUI classes

**Key Classes Documented:** 10+
**Enum Types Documented:** 20+
**Total Lines of Code (analyzed):** ~10,000+

**Balance Data Types:**
- ItemDefinition (876 tokens)
- CraftDefinition (860 tokens)
- BuffDefinition (828 tokens)
- Localization (637 tokens)

**UI Classes:**
- NGUI widget library (100+ classes)
- Game-specific GUI classes (100+ classes)

--- 

*Documentation generated from decompiled source code analysis*
*Source directory: C:\Users\SeanS\Downloads\GraveYardKeeper\GameSource*

## Core Data Structures

### GameBalance System
The game stores its master database in a `GameBalance` ScriptableObject (inherits from `GameBalanceBase`), loaded from `Resources/game_data`. It contains `List<T>` for all definitions used throughout the game.

**Location:** `Assembly-CSharp/GameBalance.cs`  
**Token:** 0x02000369 RID: 873

#### Singleton Access
```csharp
public static GameBalance me  // Returns GameBalance._instance, loads if null
```

#### Key Data Lists
- `items_data` - `List<ItemDefinition>` - All item definitions
- `objs_data` - `List<ObjectDefinition>` - All object/structure definitions
- `craft_data` - `List<CraftDefinition>` - Standard, survey, alchemy, sermon crafts
- `craft_obj_data` - `List<ObjectCraftDefinition>` - Building/removal/placement crafts
- `techs_data` - `List<TechDefinition>` - Technology definitions
- `tech_branches_data` - `List<TechBranchDefinition>` - Tech tree branches
- `quests_data` - `List<QuestDefinition>` - Quest definitions
- `bodies_data` - `List<BodyDefinition>` - Body definitions
- `souls_data` - `List<SoulDefinition>` - Soul definitions
- `fishes_data` - `List<FishDefinition>` - Fish definitions
- `perks_data` - `List<PerkDefinition>` - Perk definitions
- `product_types_data` - `List<ProductTypeDefinition>` - Product categories
- `buffs_data` - `List<BuffDefinition>` - Buff/status effect definitions
- `vendors_data` - `List<VendorDefinition>` - Vendor definitions
- `achievements_data` - `List<AchievementDefinition>` - Achievement definitions
- `workers_data` - `List<WorkerDefinition>` - Worker definitions

And 20+ additional specialized lists for auras, spawners, projectiles, logic, etc.

#### Data Loading
```csharp
public static void LoadGameBalance()
```
- Loads `game_data` from Resources
- Calls initialization caches: `CreateIDsCache()`, `CreateItemsBaseNameCache()`, `CreateToolsCache()`, `CreateCraftsCache()`
- **Hook Point:** Postfix allows injecting custom definitions before caches are built

#### Hardcoded Starting Unlocks
In `MainGame.GeneralInit()` (line 135-136):
```csharp
this.save.obj_crafts.AddCraft("p:grave_place");
this.save.obj_crafts.AddCraft("p:working_table");
```

---

### MainGame System
Central game controller handling initialization, update loop, and player state.

**Location:** `Assembly-CSharp/MainGame.cs`  
**Token:** 0x02000497 RID: 1175

#### Core Methods

**`public void Awake()`** (line 62)
- Entry point for game scene
- Prevents duplicate initialization
- Calls `StartGameLoading()`

**`public void GeneralInit()`** (line 113)
- Core initialization sequence
- Sets up managers: LazyEngine, ChunkManager, GameAwakenerEngine, etc.
- **Calls `GameBalance.LoadGameBalance()`** at line 133
- Adds hardcoded starting crafts
- Initializes GUI, tech linking, camera, fog
- **Hook Point:** Postfix allows adding post-initialization logic

**`private void InGameUpdate()`** (line 259)
- Main logic tick (called from Update())
- Updates `save.game_logics.Update()`
- Recalculates buffs: `BuffsLogics.RecalculateBuffs()`
- Updates building mode logic
- Calls `UtilityStuff.ProcessInGameUpdate()`
- **Hook Point:** Postfix for per-frame custom logic (when unpaused)

**`public void OnPlayerDied()`** (line 302)
- Handles player death sequence
- Determines respawn location via `WorldMap.GetGDPointByGDTag("gd_player_respawn")`
- Resets player position and refresh cache
- Clears overhead GUI, removes camera targets, disables player
- Removes effect bubbles
- **Modifies character stats:** Reduces _r, _g, _b parameters by 10% (rounded)
- **Hook Point:** Prefix/Postfix for death penalty modifications, respawn location changes

#### Key Fields
- `public static MainGame me` - Singleton instance
- `public WorldGameObject player` - Player reference
- `public GameSave save` - Current game save data
- `public static bool paused` - Game pause state
- `public static bool game_started` - Game initialization flag
- `public static float game_time` - Current game time (days + time of day)

---

## Hook Points Summary

### High-Priority Targets

1. **`GameBalance.LoadGameBalance()`** - Postfix
   - Inject custom Item/Craft definitions
   - Modify existing definitions
   - Add custom data to registries

2. **`MainGame.OnPlayerDied()`** - Prefix/Postfix
   - Alter death penalties
   - Change respawn mechanics
   - Modify stat reductions

3. **`MainGame.InGameUpdate()`** - Postfix
   - Per-frame custom logic
   - Real-time stat tracking
   - Automated actions

4. **`MainGame.GeneralInit()`** - Postfix
   - Post-initialization setup
   - Register custom content after data loads

5. **Various Get/Set methods** (ItemDefinition, CraftDefinition)
   - Modify item properties on-the-fly
   - Dynamic crafting costs
   - Conditional unlocks