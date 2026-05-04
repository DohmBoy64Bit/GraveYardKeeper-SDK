using System;
using System.Collections.Generic;
using UnityEngine;

namespace GraveSDK.Data.Repositories
{
    /// <summary>
    /// Manages custom interactions on WorldGameObjects.
    /// Allows modders to add new interaction events, fire events, and
    /// manipulate interaction hints on any WGO.
    /// </summary>
    public static class InteractionRepository
    {
        // Registry of custom interaction callbacks keyed by event_id
        private static readonly Dictionary<string, Action<WorldGameObject>> _customCallbacks
            = new Dictionary<string, Action<WorldGameObject>>();

        /// <summary>
        /// Adds a custom interaction event to a WorldGameObject.
        /// This will show an interaction bubble on the object.
        /// </summary>
        public static void AddInteraction(WorldGameObject wgo, string eventId)
        {
            if (wgo == null) return;
            wgo.AddInteractionEvent(eventId);
        }

        /// <summary>
        /// Removes a custom interaction event from a WorldGameObject.
        /// </summary>
        public static void RemoveInteraction(WorldGameObject wgo, string eventId)
        {
            if (wgo == null || wgo.custom_interaction_events == null) return;
            if (wgo.custom_interaction_events.Contains(eventId))
            {
                wgo.custom_interaction_events.Remove(eventId);
                wgo.RedrawBubble(null);
            }
        }

        /// <summary>
        /// Fires a named event on a WorldGameObject (triggers its FlowScript/BehaviourTree).
        /// </summary>
        public static void FireEvent(WorldGameObject wgo, string eventId, float delay = 0f)
        {
            if (wgo == null) return;
            wgo.FireEvent(eventId, delay);
        }

        /// <summary>
        /// Fires a named event with a string parameter on a WorldGameObject.
        /// </summary>
        public static void FireEvent(WorldGameObject wgo, string eventId, float delay, string param)
        {
            if (wgo == null) return;
            wgo.FireEvent(eventId, delay, param);
        }

        /// <summary>
        /// Registers a custom callback that will be invoked when a specific event_id
        /// is triggered via the SDK. Use with ProcessCustomInteraction().
        /// </summary>
        public static void RegisterCallback(string eventId, Action<WorldGameObject> callback)
        {
            _customCallbacks[eventId] = callback;
        }

        /// <summary>
        /// Unregisters a custom callback.
        /// </summary>
        public static void UnregisterCallback(string eventId)
        {
            if (_customCallbacks.ContainsKey(eventId))
                _customCallbacks.Remove(eventId);
        }

        /// <summary>
        /// Processes a custom interaction on a WGO. If a callback is registered
        /// for any of its custom_interaction_events, it will be invoked.
        /// Call this from your mod's Update loop when the player interacts.
        /// </summary>
        public static bool ProcessCustomInteraction(WorldGameObject wgo)
        {
            if (wgo == null || wgo.custom_interaction_events == null) return false;

            foreach (var eventId in wgo.custom_interaction_events)
            {
                if (_customCallbacks.TryGetValue(eventId, out var callback))
                {
                    callback.Invoke(wgo);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the list of current interaction events on a WGO.
        /// </summary>
        public static List<string> GetInteractions(WorldGameObject wgo)
        {
            if (wgo == null || wgo.custom_interaction_events == null)
                return new List<string>();
            return new List<string>(wgo.custom_interaction_events);
        }

        /// <summary>
        /// Clears all custom interaction callbacks.
        /// </summary>
        public static void ClearAllCallbacks()
        {
            _customCallbacks.Clear();
        }
    }
}
