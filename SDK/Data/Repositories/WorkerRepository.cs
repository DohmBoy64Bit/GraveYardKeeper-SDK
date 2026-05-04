using System;
using UnityEngine;

namespace GraveSDK.Data.Repositories
{
    public static class WorkerRepository
    {
        /// <summary>
        /// Sets the work efficiency of a zombie or other worker.
        /// Efficiency is usually between 0.0 and 1.0 (e.g., 0.5 = 50%).
        /// </summary>
        public static void SetEfficiency(WorldGameObject worker, float efficiency)
        {
            if (worker == null || worker.worker == null) return;
            
            // The game uses "working_k" param for efficiency
            worker.data.SetParam("working_k", efficiency);
            
            // We can also force it in the Worker class to prevent it being recalculated from skulls
            worker.worker.ForcingWorkerK(true, efficiency);
        }

        /// <summary>
        /// Converts a spawned worker back into an item and adds it to the player's inventory.
        /// </summary>
        public static bool PickUpWorker(WorldGameObject worker)
        {
            if (worker == null || worker.worker == null) return false;

            Item workerItem = worker.worker.GetOnGroundItem();
            if (workerItem != null)
            {
                if (MainGame.me.player.data.AddItem(workerItem, true))
                {
                    WorldMap.OnDestroyWGO(worker);
                    UnityEngine.Object.Destroy(worker.gameObject);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Spawns a worker from an item at the specified position.
        /// </summary>
        public static WorldGameObject DeployWorker(Item workerItem, Vector3 position)
        {
            if (workerItem == null || !workerItem.is_worker) return null;

            // Transformation logic from Worker class
            Item outItem;
            WorldGameObject outWgo;
            string error = Worker.TransformWorker(Worker.WorkerState.ItemOnGround, workerItem, null, Worker.WorkerState.WGO, out outItem, out outWgo);

            if (string.IsNullOrEmpty(error) && outWgo != null)
            {
                outWgo.tf.position = position;
                return outWgo;
            }

            Debug.LogError("DeployWorker error: " + error);
            return null;
        }

        /// <summary>
        /// Updates the visual skin of a worker based on their activity.
        /// </summary>
        public static void UpdateSkin(WorldGameObject worker, Worker.WorkerActivity activity)
        {
            if (worker == null || worker.worker == null) return;
            worker.worker.UpdateWorkerSkin(activity);
        }
    }
}
