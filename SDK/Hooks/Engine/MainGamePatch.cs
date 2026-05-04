using HarmonyLib;
using GraveSDK.UI.ModMenu;
using GraveSDK.Core;
using UnityEngine;

namespace GraveSDK.Hooks.Engine
{
    [HarmonyPatch(typeof(MainGame))]
    [HarmonyPatch("GeneralInit")]
    public static class MainGamePatch
    {
        [HarmonyPostfix]
        public static void GeneralInit_Postfix()
        {
            Debug.Log("[GraveSDK] MainGame.GeneralInit detected. Initializing SDK UI and loading mods...");
            ModMenuManager.Initialize();
            ModLoader.LoadMods();
        }
    }
}
