using System;
using System.Collections.Generic;
using UnityEngine;

namespace GraveSDK.Data.Repositories
{
    /// <summary>
    /// Global Event Repository - allows mods to subscribe to game lifecycle events
    /// without polling every frame. Hooks are triggered by Harmony patches on key methods.
    /// </summary>
    public static class GameEventRepository
    {
        // --- Day/Time Events ---
        /// <summary>Fired when a new day begins (EnvironmentEngine.OnEndOfDay).</summary>
        public static event Action<int> OnDayChanged;

        // --- NPC Events ---
        /// <summary>Fired when an NPC spawns or arrives in the world.</summary>
        public static event Action<WorldGameObject> OnNPCArrived;

        /// <summary>Fired when a body is delivered (donkey interaction event).</summary>
        public static event Action OnBodyDelivered;

        // --- Interaction Events ---
        /// <summary>Fired when the player interacts with any WorldGameObject.</summary>
        public static event Action<WorldGameObject> OnPlayerInteract;

        // --- Craft Events ---
        /// <summary>Fired when any craft completes.</summary>
        public static event Action<WorldGameObject, CraftDefinition> OnCraftCompleted;

        // --- Game State Events ---
        /// <summary>Fired when the game is saved.</summary>
        public static event Action OnGameSaved;

        /// <summary>Fired when the game finishes loading.</summary>
        public static event Action OnGameLoaded;

        /// <summary>Fired when the player dies.</summary>
        public static event Action OnPlayerDied;

        /// <summary>Fired every in-game tick (when unpaused).</summary>
        public static event Action<float> OnGameTick;

        // --- Internal Raise Methods (called by Harmony patches) ---

        /// <summary>Raises the OnDayChanged event. Called from EnvironmentEngine patch.</summary>
        public static void RaiseDayChanged(int newDay)
        {
            try { OnDayChanged?.Invoke(newDay); }
            catch (Exception ex) { Debug.LogError("[GraveSDK] OnDayChanged error: " + ex); }
        }

        /// <summary>Raises the OnNPCArrived event.</summary>
        public static void RaiseNPCArrived(WorldGameObject npc)
        {
            try { OnNPCArrived?.Invoke(npc); }
            catch (Exception ex) { Debug.LogError("[GraveSDK] OnNPCArrived error: " + ex); }
        }

        /// <summary>Raises the OnBodyDelivered event.</summary>
        public static void RaiseBodyDelivered()
        {
            try { OnBodyDelivered?.Invoke(); }
            catch (Exception ex) { Debug.LogError("[GraveSDK] OnBodyDelivered error: " + ex); }
        }

        /// <summary>Raises the OnPlayerInteract event.</summary>
        public static void RaisePlayerInteract(WorldGameObject target)
        {
            try { OnPlayerInteract?.Invoke(target); }
            catch (Exception ex) { Debug.LogError("[GraveSDK] OnPlayerInteract error: " + ex); }
        }

        /// <summary>Raises the OnCraftCompleted event.</summary>
        public static void RaiseCraftCompleted(WorldGameObject station, CraftDefinition craft)
        {
            try { OnCraftCompleted?.Invoke(station, craft); }
            catch (Exception ex) { Debug.LogError("[GraveSDK] OnCraftCompleted error: " + ex); }
        }

        /// <summary>Raises the OnGameSaved event.</summary>
        public static void RaiseGameSaved()
        {
            try { OnGameSaved?.Invoke(); }
            catch (Exception ex) { Debug.LogError("[GraveSDK] OnGameSaved error: " + ex); }
        }

        /// <summary>Raises the OnGameLoaded event.</summary>
        public static void RaiseGameLoaded()
        {
            try { OnGameLoaded?.Invoke(); }
            catch (Exception ex) { Debug.LogError("[GraveSDK] OnGameLoaded error: " + ex); }
        }

        /// <summary>Raises the OnPlayerDied event.</summary>
        public static void RaisePlayerDied()
        {
            try { OnPlayerDied?.Invoke(); }
            catch (Exception ex) { Debug.LogError("[GraveSDK] OnPlayerDied error: " + ex); }
        }

        /// <summary>Raises the OnGameTick event.</summary>
        public static void RaiseGameTick(float deltaTime)
        {
            try { OnGameTick?.Invoke(deltaTime); }
            catch (Exception ex) { Debug.LogError("[GraveSDK] OnGameTick error: " + ex); }
        }

        /// <summary>Clears all event subscriptions. Call on mod unload.</summary>
        public static void ClearAll()
        {
            OnDayChanged = null;
            OnNPCArrived = null;
            OnBodyDelivered = null;
            OnPlayerInteract = null;
            OnCraftCompleted = null;
            OnGameSaved = null;
            OnGameLoaded = null;
            OnPlayerDied = null;
            OnGameTick = null;
        }
    }
}
