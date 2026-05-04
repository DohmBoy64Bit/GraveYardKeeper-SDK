using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace GraveSDK.Hooks.Crafting
{
    /// <summary>
    /// Hook to modify craft availability and requirements
    /// Use case: Unlock all crafts, reduce costs, add custom recipes
    /// </summary>
    [HarmonyPatch(typeof(CraftDefinition))]
    [HarmonyPatch("IsLocked")]
    public static class CraftLockPatch
    {
        [HarmonyPostfix]
        public static void IsLocked_Postfix(
            CraftDefinition __instance,
            ref bool __result)
        {
            // Unlock all crafts if configured
            if (ModConfig.UnlockAllCrafts && __instance.needs_unlock)
            {
                __result = false;
                return;
            }

            // Check custom unlock conditions
            if (ModConfig.CustomUnlockConditions.TryGetValue(__instance.id, out var condition))
            {
                __result = !condition.IsUnlocked();
                return;
            }

            // Auto-unlock specific craft types
            if (ModConfig.AutoUnlockCraftTypes.Contains(__instance.craft_type))
            {
                __result = false;
            }
        }
    }

    /// <summary>
    /// Hook to modify craft requirements
    /// Use case: Reduce material costs, allow alternative materials
    /// </summary>
    [HarmonyPatch(typeof(CraftDefinition))]
    [HarmonyPatch("GetSpendTxt")]
    public static class CraftCostPatch
    {
        [HarmonyPrefix]
        public static bool GetSpendTxt_Prefix(
            CraftDefinition __instance,
            WorldGameObject wgo,
            int multiplier,
            ref string __result)
        {
            // Use custom cost calculation if configured
            if (ModConfig.UseCustomCosts &&
                ModConfig.CustomCraftCosts.TryGetValue(__instance.id, out var customCost))
            {
                __result = customCost.GetDisplayText(wgo, multiplier);
                return false; // Skip original method
            }

            return true; // Run original method
        }

        [HarmonyPostfix]
        public static void GetSpendTxt_Postfix(
            CraftDefinition __instance,
            ref string __result)
        {
            // Add discount indicator if configured
            if (ModConfig.CraftDiscounts.TryGetValue(__instance.id, out float discountPercent))
            {
                float discount = 100f - discountPercent;
                __result += $"\n[color=#00ff00][-{discountPercent}% discount][/color]";
            }
        }
    }

    /// <summary>
    /// Hook to modify craft time and speed
    /// Use case: Faster crafting, instant crafts, time scaling
    /// </summary>
    [HarmonyPatch(typeof(CraftDefinition))]
    [HarmonyPatch("craft_time")]
    public static class CraftTimePatch
    {
        [HarmonyPrefix]
        public static bool CraftTime_Getter_Prefix(
            CraftDefinition __instance,
            ref SmartExpression __result)
        {
            // Use custom craft time if configured
            if (ModConfig.CustomCraftTimes.TryGetValue(__instance.id, out float customTime))
            {
                __result = new SmartExpression { constantValue = customTime };
                return false;
            }

            // Apply global speed multiplier
            if (ModConfig.GlobalCraftSpeedMultiplier != 1f &&
                __instance.craft_time != null)
            {
                var expr = new SmartExpression();
                // Clone and modify expression
                expr.SetString(__instance.craft_time.GetString() + $" / {ModConfig.GlobalCraftSpeedMultiplier}");
                __result = expr;
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Hook to modify craft output
    /// Use case: Bonus yields, quality improvements, extra items
    /// </summary>
    [HarmonyPatch(typeof(CraftDefinition))]
    [HarmonyPatch("GetMultiqualityResult")]
    public static class CraftOutputPatch
    {
        [HarmonyPostfix]
        public static void GetMultiqualityResult_Postfix(
            CraftDefinition __instance,
            List<string> multiquality_ids,
            List<string> unlocked_perks,
            ref CraftDefinition.MultiqualityCraftResult __result)
        {
            if (__result == null) return;

            // Apply bonus to output quality
            if (ModConfig.CraftQualityBonus.TryGetValue(__instance.id, out float bonus))
            {
                __result.value_items += bonus;
                __result.SetProbabilities(
                    Mathf.Clamp(__result.value_result, 0f, 1f),
                    Mathf.Clamp(__result.value_result, 1f, 2f) - 1f,
                    Mathf.Clamp(__result.value_result, 2f, 3f) - 2f);
            }

            // Apply global bonus
            if (ModConfig.GlobalCraftQualityBonus != 0f)
            {
                __result.value_items += ModConfig.GlobalCraftQualityBonus;
                __result.SetProbabilities(
                    Mathf.Clamp(__result.value_result, 0f, 1f),
                    Mathf.Clamp(__result.value_result, 1f, 2f) - 1f,
                    Mathf.Clamp(__result.value_result, 2f, 3f) - 2f);
            }
        }
    }

    /// <summary>
    /// Hook to add custom craft validation
    /// Use case: Special requirements, conditional crafting
    /// </summary>
    [HarmonyPatch(typeof(CraftDefinition))]
    [HarmonyPatch("CanCraftMultiple")]
    public static class CraftValidationPatch
    {
        [HarmonyPostfix]
        public static void CanCraftMultiple_Postfix(
            CraftDefinition __instance,
            ref bool __result)
        {
            // Force single-craft for specific items
            if (ModConfig.ForceSingleCraft.Contains(__instance.id))
            {
                __result = false;
                return;
            }

            // Force multi-craft for specific items
            if (ModConfig.ForceMultiCraft.Contains(__instance.id))
            {
                __result = true;
                return;
            }
        }
    }

    /// <summary>
    /// Hook to intercept craft execution
    /// Use case: Custom output, side effects, logging
    /// </summary>
    [HarmonyPatch(typeof(CraftDefinition))]
    [HarmonyPatch("GetNameNonLocalized")]
    public static class CraftNamePatch
    {
        [HarmonyPostfix]
        public static void GetNameNonLocalized_Postfix(
            CraftDefinition __instance,
            ref string __result)
        {
            // Add prefix/suffix to craft names
            if (ModConfig.CraftNamePrefixes.TryGetValue(__instance.id, out string prefix))
            {
                __result = prefix + __result;
            }

            if (ModConfig.CraftNameSuffixes.TryGetValue(__instance.id, out string suffix))
            {
                __result = __result + suffix;
            }
        }
    }
}
