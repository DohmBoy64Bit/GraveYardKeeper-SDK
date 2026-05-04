using HarmonyLib;
using UnityEngine;

namespace GraveSDK.Hooks.Engine
{
    /// <summary>
    /// DLC Unlock Patch - Forces all DLC availability checks to return true.
    /// 
    /// WARNING: This only bypasses the code gate. The actual DLC asset bundles
    /// (gamedata_2.dat, gamedata_3.dat, gamedata_4.dat) must be present in the
    /// game's Data folder or the game will crash when loading DLC zones/content.
    /// 
    /// STATUS: DISABLED by default. Set Enabled = true to activate.
    /// </summary>
    [HarmonyPatch(typeof(DLCEngine))]
    [HarmonyPatch("IsDLCAvailable")]
    public static class DLCUnlockPatch
    {
        /// <summary>
        /// Master toggle. Set to true to force all DLCs as available.
        /// </summary>
        public static bool Enabled = false;

        /// <summary>
        /// Individual DLC toggles (only used when Enabled = true).
        /// </summary>
        public static bool UnlockStrangerSins = true;   // gamedata_2.dat
        public static bool UnlockGameOfCrone = true;     // gamedata_3.dat
        public static bool UnlockBetterSaveSoul = true;  // gamedata_4.dat

        [HarmonyPrefix]
        public static bool IsDLCAvailable_Prefix(DLCEngine.DLCVersion dlc_version, ref bool __result)
        {
            if (!Enabled) return true; // Run original method

            switch (dlc_version)
            {
                case DLCEngine.DLCVersion.Stories:
                    __result = UnlockStrangerSins;
                    break;
                case DLCEngine.DLCVersion.Refugees:
                    __result = UnlockGameOfCrone;
                    break;
                case DLCEngine.DLCVersion.Souls:
                    __result = UnlockBetterSaveSoul;
                    break;
                default:
                    return true; // Run original for unknown versions
            }

            if (__result)
                Debug.Log($"[GraveSDK] DLC Unlocked: {dlc_version}");

            return false; // Skip original method
        }
    }
}
