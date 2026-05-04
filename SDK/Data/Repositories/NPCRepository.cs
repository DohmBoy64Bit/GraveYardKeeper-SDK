using System.Collections.Generic;
using System.Linq;
using GraveSDK.Data.Models;

namespace GraveSDK.Data.Repositories
{
    public class NPCRepository
    {
        private readonly Dictionary<string, NPCModel> _cache;

        public NPCRepository()
        {
            _cache = new Dictionary<string, NPCModel>();
        }

        public NPCModel GetNPC(string npcId)
        {
            if (string.IsNullOrEmpty(npcId)) return null;
            if (_cache.TryGetValue(npcId, out var cached)) return cached;

            ObjectDefinition def = GameBalance.me?.GetData<ObjectDefinition>(npcId);
            if (def == null || def.type != ObjectDefinition.ObjType.NPC) return null;

            var model = ConvertToModel(def);
            _cache[npcId] = model;
            return model;
        }

        public List<NPCModel> GetAllNPCs()
        {
            if (GameBalance.me?.objs_data == null) return new List<NPCModel>();
            return GameBalance.me.objs_data
                .Where(d => d.type == ObjectDefinition.ObjType.NPC)
                .Select(d => GetNPC(d.id))
                .Where(m => m != null)
                .ToList();
        }

        private NPCModel ConvertToModel(ObjectDefinition def)
        {
            return new NPCModel
            {
                Id = def.id,
                DisplayName = GJL.L(def.id),
                Alias = def.npc_alias,
                CustomHeadSprite = def.custom_head_spr,
                IsInNPCList = def.npc_in_list,
                IsRelationVisible = def.IsRelationVisible(),
                Groups = new List<string>(def.object_groups.Select(g => g.id))
            };
        }

        public void ClearCache()
        {
            _cache.Clear();
        }
    }
}
