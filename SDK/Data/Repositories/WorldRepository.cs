using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GraveSDK.Data.Repositories
{
    public class WorldRepository
    {
        /// <summary>
        /// Spawns a game object at the specified position.
        /// </summary>
        public WorldGameObject SpawnObject(string id, Vector3 position, string customTag = "")
        {
            var go = new GameObject("TempPos");
            go.transform.position = position;
            var wgo = GS.Spawn(id, go.transform, customTag);
            Object.Destroy(go);
            return wgo;
        }

        /// <summary>
        /// Finds a WorldGameObject by its custom tag.
        /// </summary>
        public WorldGameObject FindByTag(string tag)
        {
            return WorldMap.objs.FirstOrDefault(o => o.custom_tag == tag);
        }

        /// <summary>
        /// Finds all WorldGameObjects with a specific custom tag.
        /// </summary>
        public List<WorldGameObject> FindAllByTag(string tag)
        {
            return WorldMap.objs.Where(o => o.custom_tag == tag).ToList();
        }

        /// <summary>
        /// Finds a WorldGameObject by its unique ID.
        /// </summary>
        public WorldGameObject FindByUniqueID(long uniqueID)
        {
            return WorldMap.objs.FirstOrDefault(o => o.unique_id == uniqueID);
        }

        /// <summary>
        /// Finds an NPC by their object ID (e.g., "gerry").
        /// </summary>
        public WorldGameObject GetNPC(string id)
        {
            return WorldMap.objs.FirstOrDefault(o => o.obj_id == id && o.obj_def.IsNPC());
        }

        /// <summary>
        /// Gets all active NPCs in the world.
        /// </summary>
        public List<WorldGameObject> GetAllNPCs()
        {
            return WorldMap.objs.Where(o => o.obj_def.IsNPC()).ToList();
        }
        
        public List<WorldGameObject> GetAllObjects()
        {
            return WorldMap.objs;
        }

        /// <summary>
        /// Gets the nearest object of a certain type to a position.
        /// </summary>
        public WorldGameObject GetNearestObject(Vector3 position, string objId = null)
        {
            var objects = WorldMap.objs;
            if (!string.IsNullOrEmpty(objId))
            {
                objects = objects.Where(o => o.obj_id == objId).ToList();
            }
            
            WorldGameObject nearest = null;
            float minDist = float.MaxValue;
            
            foreach (var obj in objects)
            {
                float dist = Vector3.Distance(position, obj.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = obj;
                }
            }
            
            return nearest;
        }
    }
}
