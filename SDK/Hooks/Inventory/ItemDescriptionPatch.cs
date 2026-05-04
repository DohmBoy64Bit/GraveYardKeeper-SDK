using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace GraveSDK.Hooks.Inventory
{
    /// <summary>
    /// Hook to intercept and modify item description display
    /// Use case: Add mod metadata, enhance tooltips, inject custom data
    /// </summary>
    [HarmonyPatch(typeof(ItemDefinition))]
    [HarmonyPatch("GetItemDescription")]
    public static class ItemDescriptionPatch
    {
        /// <summary>
        /// Prefix hook - runs BEFORE original method
        /// Can modify parameters or skip original execution
        /// </summary>
        [HarmonyPrefix]
        public static bool GetItemDescription_Prefix(
            ItemDefinition __instance,
            Item real_item,
            ref bool __runOriginal)
        {
            // Always run original - we'll modify after
            __runOriginal = true;
            return true;
        }

        /// <summary>
        /// Postfix hook - runs AFTER original method
        /// Can modify the result
        /// </summary>
        [HarmonyPostfix]
        public static void GetItemDescription_Postfix(
            ItemDefinition __instance,
            ref string __result,
            Item real_item)
        {
            // Check if mod wants to add extra info
            if (ModConfig.ShowItemMetadata)
            {
                string metadata = BuildMetadataString(__instance, real_item);
                if (!string.IsNullOrEmpty(metadata))
                {
                    __result += "\n\n" + metadata;
                }
            }

            // Check for custom item overrides
            if (ModConfig.CustomItemDescriptions.TryGetValue(__instance.id, out string customDesc))
            {
                __result = customDesc;
            }
        }

        /// <summary>
        /// Build metadata string to append to description
        /// </summary>
        private static string BuildMetadataString(
            ItemDefinition item,
            Item realItem)
        {
            var lines = new List<string>();
            
            // Add internal ID
            lines.Add($"[size=8][color=grey]ID: {item.id}[/color][/size]");

            // Add quality info
            if (item.quality_type == ItemDefinition.QualityType.Stars && item.quality > 0)
            {
                lines.Add($"[size=8]Quality: {item.quality:0.#}★[/size]");
            }

            // Add type info
            lines.Add($"[size=8]Type: {item.type}[/size]");

            // Add durability info
            if (item.has_durability)
            {
                float dur = realItem?.durability ?? item.durability_decrease;
                lines.Add($"[size=8]Durability: {dur:0.#}[/size]");
            }

            // Add stack info
            if (item.stack_count > 1)
            {
                lines.Add($"[size=8]Stack: {item.stack_count}[/size]");
            }

            // Add tool efficiency
            if (item.is_tool)
            {
                int eff = GameBalance.me?.GetToolEfficiencyPercent(item) ?? 0;
                if (eff > 0)
                {
                    lines.Add($"[size=8]Efficiency: {eff}%[/size]");
                }
            }

            return string.Join("\n", lines);
        }
    }

    /// <summary>
    /// Hook to intercept item name display
    /// Use case: Rename items, add prefixes/suffixes, localization overrides
    /// </summary>
    [HarmonyPatch(typeof(ItemDefinition))]
    [HarmonyPatch("GetItemName")]
    public static class ItemNamePatch
    {
        [HarmonyPostfix]
        public static void GetItemName_Postfix(
            ItemDefinition __instance,
            bool localized,
            ref string __result)
        {
            // Add prefix if configured
            if (ModConfig.ItemPrefixes.TryGetValue(__instance.id, out string prefix))
            {
                __result = $"{prefix}{__result}";
            }

            // Add suffix if configured
            if (ModConfig.ItemSuffixes.TryGetValue(__instance.id, out string suffix))
            {
                __result = $"{__result}{suffix}";
            }

            // Override completely if configured
            if (ModConfig.ItemNameOverrides.TryGetValue(__instance.id, out string overrideName))
            {
                __result = overrideName;
            }
        }
    }

    /// <summary>
    /// Hook to intercept item usage
    /// Use case: Custom item behaviors, usage logging, effect modification
    /// </summary>
    [HarmonyPatch(typeof(ItemDefinition))]
    [HarmonyPatch("GetTooltipData")]
    public static class ItemTooltipPatch
    {
        [HarmonyPostfix]
        public static void GetTooltipData_Postfix(
            ItemDefinition __instance,
            Item item,
            bool full_detail,
            ref List<BubbleWidgetData> __result)
        {
            if (!ModConfig.ShowExtendedTooltips) return;

            // Add separator
            __result.Add(new BubbleWidgetSeparatorData());

            // Add custom sections
            if (ModConfig.ExtendedTooltipSections.TryGetValue(__instance.id, out var sections))
            {
                foreach (var section in sections)
                {
                    __result.Add(new BubbleWidgetTextData(
                        section,
                        UITextStyles.TextStyle.TinyDescription,
                        NGUIText.Alignment.Center,
                        -1));
                }
            }

            // Add debug info in dev mode
            if (ModConfig.DevMode)
            {
                __result.Add(new BubbleWidgetSeparatorData());
                __result.Add(new BubbleWidgetTextData(
                    $"[color=#ff8800]Debug ID: {__instance.id}[/color]",
                    UITextStyles.TextStyle.TinyDescription,
                    NGUIText.Alignment.Left,
                    -1));
            }
        }
    }
}
