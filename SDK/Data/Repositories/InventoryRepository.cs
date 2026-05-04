using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GraveSDK.Data.Repositories
{
    public class InventoryRepository
    {

        /// <summary>
        /// Gets the player's multi-inventory (bag + equipped + toolbelt).
        /// </summary>
        public MultiInventory GetPlayerInventory()
        {
            return MainGame.me.player.GetMultiInventory(null, "", MultiInventory.PlayerMultiInventory.IncludePlayer, false, true, true);
        }

        /// <summary>
        /// Adds an item to the player's inventory.
        /// </summary>
        public bool AddItem(string id, int count = 1)
        {
            var inv = GetPlayerInventory();
            return inv.AddItem(id, count);
        }

        /// <summary>
        /// Removes an item from the player's inventory.
        /// </summary>
        public bool RemoveItem(string id, int count = 1)
        {
            var inv = GetPlayerInventory();
            return inv.RemoveItem(id, count);
        }

        /// <summary>
        /// Checks if the player has a certain amount of an item.
        /// </summary>
        public bool HasItem(string id, int count = 1)
        {
            var inv = GetPlayerInventory();
            return inv.GetTotalCount(id) >= count;
        }

        /// <summary>
        /// Gets the total count of an item across all player inventories.
        /// </summary>
        public int GetItemCount(string id)
        {
            var inv = GetPlayerInventory();
            return inv.GetTotalCount(id);
        }

        /// <summary>
        /// Finds the nearest container (chest/storage) that the player can interact with.
        /// </summary>
        public WorldGameObject GetNearestContainer()
        {
            var playerPos = MainGame.me.player.transform.position;
            return WorldMap.objs
                .Where(o => o.obj_def != null && o.obj_def.inventory_size > 0)
                .OrderBy(o => Vector3.Distance(playerPos, o.transform.position))
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the inventory of a specific container object.
        /// </summary>
        public MultiInventory GetContainerInventory(WorldGameObject container)
        {
            if (container == null || container.obj_def.inventory_size <= 0) return null;
            return container.GetMultiInventory(null, "", MultiInventory.PlayerMultiInventory.ExcludePlayer, true, false, false);
        }
    }
}
