using System.Collections.Generic;
using System.Linq;
using GraveSDK.Data.Models;

namespace GraveSDK.Data.Repositories
{
    public class ObjectRepository
    {
        private readonly Dictionary<string, ObjectModel> _cache;

        public ObjectRepository()
        {
            _cache = new Dictionary<string, ObjectModel>();
        }

        public ObjectModel GetObject(string objId)
        {
            if (string.IsNullOrEmpty(objId)) return null;
            if (_cache.TryGetValue(objId, out var cached)) return cached;

            ObjectDefinition def = GameBalance.me?.GetData<ObjectDefinition>(objId);
            if (def == null) return null;

            var model = ConvertToModel(def);
            _cache[objId] = model;
            return model;
        }

        public List<ObjectModel> GetAllObjects()
        {
            if (GameBalance.me?.objs_data == null) return new List<ObjectModel>();
            return GameBalance.me.objs_data
                .Select(d => GetObject(d.id))
                .Where(m => m != null)
                .ToList();
        }

        private ObjectModel ConvertToModel(ObjectDefinition def)
        {
            return new ObjectModel
            {
                Id = def.id,
                DisplayName = GJL.L(def.id),
                Type = (GraveSDK.Data.Models.ObjectType)def.type,
                MaxHp = def.hp?.EvaluateFloat(null, null) ?? 0f,
                InventorySize = def.inventory_size,
                IsNPC = def.IsNPC(),
                IsMob = def.IsMob(),
                CustomIcon = def.custom_icon,
                Groups = new List<string>(def.object_groups.Select(g => g.id))
            };
        }

        public void ClearCache()
        {
            _cache.Clear();
        }
    }
}
