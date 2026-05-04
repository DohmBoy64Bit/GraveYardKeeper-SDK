using System;
using UnityEngine;

namespace GraveSDK.Data.Repositories
{
    public class SaveRepository
    {
        /// <summary>
        /// Triggers a manual save of the current game state to the current slot.
        /// </summary>
        public void SaveGame(Action onComplete = null)
        {
            if (MainGame.me.save_slot == null)
            {
                Debug.LogWarning("[GraveSDK] Cannot save: No active save slot found.");
                return;
            }

            PlatformSpecific.SaveGame(MainGame.me.save_slot, MainGame.me.save, (slot) =>
            {
                Debug.Log("[GraveSDK] Game saved successfully.");
                onComplete?.Invoke();
            });
        }

        /// <summary>
        /// Gets a game flag (parameter) from the player data.
        /// </summary>
        public float GetFlag(string flagId, float defaultValue = 0f)
        {
            return MainGame.me.player.GetParam(flagId, defaultValue);
        }

        /// <summary>
        /// Sets a game flag (parameter) on the player data.
        /// </summary>
        public void SetFlag(string flagId, float value)
        {
            MainGame.me.player.SetParam(flagId, value);
        }

        /// <summary>
        /// Gets an integer game flag.
        /// </summary>
        public int GetFlagInt(string flagId)
        {
            return MainGame.me.player.GetParamInt(flagId);
        }

        /// <summary>
        /// Increases a game flag by a certain amount.
        /// </summary>
        public void AddToFlag(string flagId, float amount)
        {
            MainGame.me.player.AddToParams(flagId, amount);
        }

        /// <summary>
        /// Checks if a tech is unlocked.
        /// </summary>
        public bool IsTechUnlocked(string techId)
        {
            return MainGame.me.save.unlocked_techs.Contains(techId);
        }

        /// <summary>
        /// Unlocks a tech.
        /// </summary>
        public void UnlockTech(string techId)
        {
            MainGame.me.save.UnlockTech(techId);
        }

        /// <summary>
        /// Checks if a craft recipe is unlocked.
        /// </summary>
        public bool IsCraftUnlocked(string craftId)
        {
            return MainGame.me.save.unlocked_crafts.Contains(craftId);
        }

        /// <summary>
        /// Unlocks a craft recipe.
        /// </summary>
        public void UnlockCraft(string craftId)
        {
            MainGame.me.save.UnlockCraft(craftId);
        }
    }
}
