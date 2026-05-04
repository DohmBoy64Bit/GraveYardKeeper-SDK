using HarmonyLib;
using GraveSDK.Data.Repositories;
using UnityEngine;

namespace GraveSDK.Hooks.Events
{
    /// <summary>
    /// Harmony patch on EnvironmentEngine.OnEndOfDay to fire the SDK's OnDayChanged event.
    /// </summary>
    [HarmonyPatch(typeof(EnvironmentEngine))]
    [HarmonyPatch("OnEndOfDay")]
    public static class EndOfDayPatch
    {
        [HarmonyPostfix]
        public static void OnEndOfDay_Postfix()
        {
            if (MainGame.me?.save != null)
            {
                GameEventRepository.RaiseDayChanged(MainGame.me.save.day);
            }
        }
    }

    /// <summary>
    /// Harmony patch on MainGame.InGameUpdate to fire the SDK's OnGameTick event.
    /// </summary>
    [HarmonyPatch(typeof(MainGame))]
    [HarmonyPatch("InGameUpdate")]
    public static class GameTickPatch
    {
        [HarmonyPostfix]
        public static void InGameUpdate_Postfix()
        {
            GameEventRepository.RaiseGameTick(Time.deltaTime);
        }
    }

    /// <summary>
    /// Harmony patch on MainGame.OnPlayerDied to fire the SDK's OnPlayerDied event.
    /// </summary>
    [HarmonyPatch(typeof(MainGame))]
    [HarmonyPatch("OnPlayerDied")]
    public static class PlayerDiedEventPatch
    {
        [HarmonyPostfix]
        public static void OnPlayerDied_Postfix()
        {
            GameEventRepository.RaisePlayerDied();
        }
    }

    /// <summary>
    /// Harmony patch on GameSave.PrepareForSave to fire the SDK's OnGameSaved event.
    /// </summary>
    [HarmonyPatch(typeof(GameSave))]
    [HarmonyPatch("PrepareForSave")]
    public static class GameSavedPatch
    {
        [HarmonyPostfix]
        public static void PrepareForSave_Postfix()
        {
            GameEventRepository.RaiseGameSaved();
        }
    }

    /// <summary>
    /// Harmony patch on GameSave.PrepareAfterLoad to fire the SDK's OnGameLoaded event.
    /// </summary>
    [HarmonyPatch(typeof(GameSave))]
    [HarmonyPatch("PrepareAfterLoad")]
    public static class GameLoadedPatch
    {
        [HarmonyPostfix]
        public static void PrepareAfterLoad_Postfix()
        {
            GameEventRepository.RaiseGameLoaded();
        }
    }

    /// <summary>
    /// Harmony patch on InteractionComponent.Interact to fire the SDK's OnPlayerInteract event.
    /// </summary>
    [HarmonyPatch(typeof(InteractionComponent))]
    [HarmonyPatch("Interact", new System.Type[] { typeof(bool) })]
    public static class PlayerInteractPatch
    {
        [HarmonyPostfix]
        public static void Interact_Postfix(InteractionComponent __instance, bool __result)
        {
            if (__result && __instance.nearest != null)
            {
                GameEventRepository.RaisePlayerInteract(__instance.nearest);
            }
        }
    }

    /// <summary>
    /// Harmony patch on CraftComponent to fire the SDK's OnCraftCompleted event.
    /// </summary>
    [HarmonyPatch(typeof(CraftComponent))]
    [HarmonyPatch("CraftReally")]
    public static class CraftCompletedPatch
    {
        [HarmonyPostfix]
        public static void CraftReally_Postfix(CraftComponent __instance)
        {
            if (__instance.current_craft != null)
            {
                GameEventRepository.RaiseCraftCompleted(__instance.wgo, __instance.current_craft);
            }
        }
    }
}
