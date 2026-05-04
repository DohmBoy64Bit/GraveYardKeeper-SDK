using UnityEngine;
using GraveSDK.Core;

namespace GraveSDK.UI.ModMenu
{
    public static class ModMenuManager
    {
        private static GameObject _modMenuObject;
        private static ModMenuGUI _modMenuGUI;

        public static void Initialize()
        {
            if (_modMenuObject != null) return;

            Debug.Log("[GraveSDK] Initializing Mod Menu...");
            
            ConfigManager.Load();
            
            _modMenuObject = new GameObject("GraveSDK_ModMenu");
            Object.DontDestroyOnLoad(_modMenuObject);
            
            _modMenuGUI = _modMenuObject.AddComponent<ModMenuGUI>();
        }

        public static void Shutdown()
        {
            if (_modMenuObject != null)
            {
                Object.Destroy(_modMenuObject);
                _modMenuObject = null;
                _modMenuGUI = null;
            }
        }
    }
}
