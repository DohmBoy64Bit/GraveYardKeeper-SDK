using HarmonyLib;
using GraveSDK.Data.Repositories;

namespace GraveSDK.Hooks.Localization
{
    [HarmonyPatch(typeof(global::Localization))]
    [HarmonyPatch("Get")]
    public static class LocalizationPatch
    {
        // Custom dictionary for mod-injected strings to avoid polluting the main game dictionary
        // though the repository also provides a way to inject directly into the game's dict.
        
        [HarmonyPrefix]
        public static bool Get_Prefix(string key, bool warnIfMissing, ref string __result)
        {
            // If the key starts with "mod_", we could handle it specially if we wanted a separate system.
            // For now, the game's Localization.mDictionary is public enough to use via the repository.
            
            // This hook is mainly here to allow for advanced interception, 
            // like dynamic string replacement or live-reloading mod translations.
            
            return true; // Continue with original logic
        }
    }
}
