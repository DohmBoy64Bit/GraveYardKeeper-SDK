using BepInEx;
using BepInEx.Unity.Mono;
using HarmonyLib;
using GraveSDK.UI.ModMenu;
using GraveSDK.Tools;
using UnityEngine;

namespace GraveSDK.Core
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Initialize Config
            ConfigManager.Load();

            // Patch everything
            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            // Setup Mod Menu
            ModMenuManager.Initialize();

            // Attach GameData Dumper (F10 to dump)
            gameObject.AddComponent<GameDataDumper>();

            // Load external mods
            ModLoader.LoadMods();

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }

    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "com.grave.sdk";
        public const string PLUGIN_NAME = "GraveYardKeeperSDK";
        public const string PLUGIN_VERSION = "1.0.0";
    }
}
