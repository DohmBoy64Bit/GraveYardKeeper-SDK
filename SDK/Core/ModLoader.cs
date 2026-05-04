using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace GraveSDK.Core
{
    public static class ModLoader
    {
        private static string ModsPath => Path.Combine(Application.persistentDataPath, "Mods");
        private static List<Assembly> _loadedModAssemblies = new List<Assembly>();

        public static void LoadMods()
        {
            if (!Directory.Exists(ModsPath))
            {
                Directory.CreateDirectory(ModsPath);
                Debug.Log("[GraveSDK] Created Mods folder at: " + ModsPath);
                return;
            }

            string[] dlls = Directory.GetFiles(ModsPath, "*.dll");
            foreach (string dll in dlls)
            {
                try
                {
                    Debug.Log("[GraveSDK] Loading mod: " + Path.GetFileName(dll));
                    Assembly assembly = Assembly.LoadFrom(dll);
                    _loadedModAssemblies.Add(assembly);
                    
                    // Look for classes that implement IMod interface or have a specific attribute
                    // For now, let's just look for any class with an 'Initialize' static method
                    foreach (Type type in assembly.GetTypes())
                    {
                        MethodInfo initMethod = type.GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static);
                        if (initMethod != null)
                        {
                            Debug.Log($"[GraveSDK] Calling Initialize on {type.FullName}");
                            initMethod.Invoke(null, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[GraveSDK] Failed to load mod {dll}: {ex.Message}");
                }
            }
        }
    }
}
