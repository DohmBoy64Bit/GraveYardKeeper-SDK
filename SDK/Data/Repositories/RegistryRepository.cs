using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GraveSDK.Data.Repositories
{
    public class RegistryRepository
    {
        /// <summary>
        /// Registers a custom item definition into the game balance.
        /// </summary>
        public void RegisterItem(ItemDefinition item)
        {
            if (item == null) return;
            
            string error = GameBalance.me.AddData(item);
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError($"[GraveSDK] Failed to register item {item.id}: {error}");
            }
            else
            {
                GameBalance.me.CreateIDsCache();
                InvokePrivateMethod(GameBalance.me, "CreateItemsBaseNameCache");
                InvokePrivateMethod(GameBalance.me, "CreateToolsCache");
                Debug.Log($"[GraveSDK] Registered item: {item.id}");
            }
        }

        /// <summary>
        /// Registers a custom craft definition into the game balance.
        /// </summary>
        public void RegisterCraft(CraftDefinition craft)
        {
            if (craft == null) return;
            
            string error = GameBalance.me.AddData(craft);
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError($"[GraveSDK] Failed to register craft {craft.id}: {error}");
            }
            else
            {
                GameBalance.me.CreateIDsCache();
                InvokePrivateMethod(GameBalance.me, "CreateCraftsCache");
                Debug.Log($"[GraveSDK] Registered craft: {craft.id}");
            }
        }

        private void InvokePrivateMethod(object obj, string methodName)
        {
            var method = obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (method != null)
            {
                method.Invoke(obj, null);
            }
            else
            {
                Debug.LogWarning($"[GraveSDK] Private method {methodName} not found in {obj.GetType().Name}");
            }
        }
    }
}
