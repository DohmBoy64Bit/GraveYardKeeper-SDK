using System.Collections.Generic;
using System.Linq;
using GraveSDK.Data.Models;

namespace GraveSDK.Data.Repositories
{
    public class TechRepository
    {
        private readonly Dictionary<string, TechModel> _cache;

        public TechRepository()
        {
            _cache = new Dictionary<string, TechModel>();
        }

        public TechModel GetTech(string techId)
        {
            if (string.IsNullOrEmpty(techId)) return null;
            if (_cache.TryGetValue(techId, out var cached)) return cached;

            TechDefinition def = GameBalance.me?.GetData<TechDefinition>(techId);
            if (def == null) return null;

            var model = ConvertToModel(def);
            _cache[techId] = model;
            return model;
        }

        public List<TechModel> GetAllTechs()
        {
            if (GameBalance.me?.techs_data == null) return new List<TechModel>();
            return GameBalance.me.techs_data
                .Select(d => GetTech(d.id))
                .Where(m => m != null)
                .ToList();
        }

        private TechModel ConvertToModel(TechDefinition def)
        {
            return new TechModel
            {
                Id = def.id,
                DisplayName = GJL.L(def.id),
                IconName = def.icon,
                BranchType = def.branch_type,
                IsHidden = def.hidden,
                IsInvisible = def.invisible,
                Status = (GraveSDK.Data.Models.TechStatus)def.GetState(),
                ParentIds = new List<string>(def.parents.Select(p => p.id)),
                UnlockIds = new List<string>(def.GetUnlocksList().Select(u => u.id))
            };
        }

        public void ClearCache()
        {
            _cache.Clear();
        }
    }
}
