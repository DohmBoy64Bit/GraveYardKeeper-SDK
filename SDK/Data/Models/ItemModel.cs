using System;
using System.Collections.Generic;
using System.Linq;

namespace GraveSDK.Data.Models
{
    /// <summary>
    /// Type-safe model for game items
    /// </summary>
    public class ItemModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public ItemType Type { get; set; }
        public float Quality { get; set; }
        public int StackCount { get; set; }
        public float BasePrice { get; set; }
        public bool HasDurability { get; set; }
        public float DurabilityDecrease { get; set; }
        public Dictionary<string, float> Parameters { get; set; }
        public List<string> OnUseEffects { get; set; }
        public bool CanBeUsed { get; set; }
        public bool IsTool { get; set; }
        public bool IsWeapon { get; set; }
        public bool IsEquipment { get; set; }
    }

    /// <summary>
    /// Type-safe model for crafting recipes
    /// </summary>
    public class CraftModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public List<CraftIngredient> Needs { get; set; }
        public List<CraftResult> Output { get; set; }
        public float Difficulty { get; set; }
        public List<string> RequiredPerks { get; set; }
        public List<string> LinkedBuffs { get; set; }
        public bool IsLocked { get; set; }
        public bool NeedsUnlock { get; set; }
        public bool CanAutoCraft { get; set; }
        public bool IsAuto { get; set; }
        public float EnergyCost { get; set; }
        public float TimeCost { get; set; }
        public float GratitudePointsCost { get; set; }
        public string TabId { get; set; }
        public CraftType CraftType { get; set; }
        public CraftSubType SubType { get; set; }
        public bool CanCraftMultiple { get; set; }
    }

    /// <summary>
    /// Type-safe model for buffs/status effects
    /// </summary>
    public class BuffModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public float Duration { get; set; }
        public bool IsHidden { get; set; }
        public Dictionary<string, float> ResourceEffects { get; set; }
        public string CustomIcon { get; set; }
        public float TickPeriod { get; set; }
        public bool DoNotShowTimer { get; set; }
        public float CraftQualityBonus { get; set; }
        public BuffOverlayType OverlayType { get; set; }
    }

    /// <summary>
    /// Craft recipe ingredient
    /// </summary>
    public class CraftIngredient
    {
        public string ItemId { get; set; }
        public int Count { get; set; }
        public bool IsMultiquality { get; set; }
    }

    /// <summary>
    /// Craft recipe result
    /// </summary>
    public class CraftResult
    {
        public string ItemId { get; set; }
        public int Count { get; set; }
        public bool IsMultiquality { get; set; }
    }

    /// <summary>
    /// Mirror of ItemDefinition.ItemType
    /// </summary>
    public enum ItemType
    {
        None = 0,
        Axe = 1,
        Pickaxe = 2,
        Shovel = 3,
        Sword = 4,
        Hammer = 5,
        FishingRod = 6,
        Torch = 9,
        Hand = 10,
        Item = 11,
        HeadArmor = 12,
        BodyArmor = 13,
        Preach = 20,
        Bait = 31,
        Crate = 50,
        Rat = 60,
        RatBuff = 61,
        GraveStone = 101,
        GraveFence = 102,
        GraveCover = 103,
        Body = 200,
        BodyHead = 201,
        BodyBody = 202,
        BodyArmR = 203,
        BodyArmL = 204,
        BodyLegR = 205,
        BodyLegL = 206,
        BodyHeadPart = 210,
        BodyBodyPart = 220,
        BodyArmPart = 230,
        BodyLegPart = 250,
        BodyUniversalPart = 270,
        SoulBodyPart = 271,
        Soul = 280,
        ZombieWorker = 300,
        Bag = 400
    }

    /// <summary>
    /// Mirror of CraftDefinition.CraftType
    /// </summary>
    public enum CraftType
    {
        None = 0,
        ResourcesBasedCraft = 1,
        Survey = 2,
        MixedCraft = 3,
        Fixing = 4,
        AlchemyDecompose = 5,
        PrayCraft = 6,
        RatBuff = 7,
        RefugeeCampCraft = 8
    }

    /// <summary>
    /// Mirror of CraftDefinition.CraftSubType
    /// </summary>
    public enum CraftSubType
    {
        None = 0,
        Alchemy = 1,
        SurveySciencePoints = 2
    }

    /// <summary>
    /// Mirror of BuffDefinition.BuffOverlayType
    /// </summary>
    public enum BuffOverlayType
    {
        Set = 0,
        Add = 1
    }

    /// <summary>
    /// Mirror of ItemDefinition.QualityType
    /// </summary>
    public enum QualityType
    {
        Default = 0,
        Stars = 1
    }

    /// <summary>
    /// Mirror of ItemDefinition.AlchemyType
    /// </summary>
    public enum AlchemyType
    {
        None = 0,
        Powder = 1,
        Fluid = 2,
        Essence = 3,
        Universal = 9
    }

    /// <summary>
    /// Mirror of ItemDefinition.BagType
    /// </summary>
    public enum BagType
    {
        None = 0,
        Universal = 1,
        Alchemy = 2,
        Farming = 3,
        Fishing = 4,
        Tools = 5,
        Potions = 6,
        Builder = 7,
        Food = 8
    }

    /// <summary>
    /// Mirror of ItemDefinition.EquipmentType
    /// </summary>
    public enum EquipmentType
    {
        None = 0,
        Axe = 1,
        Pickaxe = 2,
        Shovel = 3,
        Sword = 4,
        Hammer = 5,
        FishingRod = 6,
        HeadArmor = 12,
        BodyArmor = 13
    }
}
