using HarmonyLib;
using UnityEngine;

namespace GraveSDK.Hooks.UI
{
    [HarmonyPatch(typeof(UIRoot))]
    [HarmonyPatch("Broadcast")]
    public static class UIRootPatch
    {
        public delegate void UIEventHandler(string funcName);
        public static event UIEventHandler OnUIEvent;

        [HarmonyPrefix]
        public static void Broadcast_Prefix(string funcName)
        {
            // This allows the SDK to react to game-wide UI events
            // such as "OnLocalize", "OnPause", "OnUnpause", etc.
            OnUIEvent?.Invoke(funcName);
            
            if (funcName == "OnLocalize")
            {
                Debug.Log("[GraveSDK] UI Language change detected via UIRoot.Broadcast");
            }
        }
    }
}
