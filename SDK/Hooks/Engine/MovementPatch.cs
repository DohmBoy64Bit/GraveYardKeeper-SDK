using HarmonyLib;
using GraveSDK.Core;
using UnityEngine;

namespace GraveSDK.Hooks.Engine
{
    [HarmonyPatch(typeof(MovementComponent))]
    [HarmonyPatch("UpdateMovement", new System.Type[] { typeof(Vector2), typeof(float) })]
    public static class MovementPatch
    {
        [HarmonyPrefix]
        public static void UpdateMovement_Prefix(MovementComponent __instance, ref float delta_time)
        {
            // We can't easily change the speed inside the method without prefixing and rewriting or using transpilers.
            // But we can multiply the delta_time to fake a speed increase, 
            // though that might affect other things like animations if they depend on delta_time here.
            
            // Actually, the method calculates 'num' (speed).
            // Let's see if we can use a Transpiler later.
            // For now, let's try to just scale delta_time if it's the player.
            
            if (__instance.wgo.is_player && ConfigManager.Current.MovementSpeedMultiplier != 1.0f)
            {
                delta_time *= ConfigManager.Current.MovementSpeedMultiplier;
            }
        }
    }
}
