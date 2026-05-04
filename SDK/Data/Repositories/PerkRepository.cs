using System.Collections.Generic;
using System.Linq;
using GraveSDK.Data.Models;

namespace GraveSDK.Data.Repositories
{
    public class PerkRepository
    {
        private readonly Dictionary<string, PerkModel> _cache;

        public PerkRepository()
        {
            _cache = new Dictionary<string, PerkModel>();
        }

        public PerkModel GetPerk(string perkId)
        {
            if (string.IsNullOrEmpty(perkId)) return null;
            if (_cache.TryGetValue(perkId, out var cached)) return cached;

            PerkDefinition def = GameBalance.me?.GetData<PerkDefinition>(perkId);
            if (def == null) return null;

            var model = ConvertToModel(def);
            _cache[perkId] = model;
            return model;
        }

        public List<PerkModel> GetAllPerks()
        {
            if (GameBalance.me?.perks_data == null) return new List<PerkModel>();
            return GameBalance.me.perks_data
                .Select(d => GetPerk(d.id))
                .Where(m => m != null)
                .ToList();
        }

        public List<PerkModel> GetUnlockedPerks()
        {
            return GetAllPerks().Where(p => p.IsUnlocked).ToList();
        }

        private PerkModel ConvertToModel(PerkDefinition def)
        {
            return new PerkModel
            {
                Id = def.id,
                DisplayName = GJL.L(def.id),
                Description = def.GetDescriptionIfExists(),
                IconName = def.GetIcon(),
                Stars = def.stars,
                ShowInUI = def.show,
                IsUnlocked = MainGame.me?.save?.unlocked_perks?.Contains(def.id) ?? false
            };
        }

        public void ClearCache()
        {
            _cache.Clear();
        }
    }
}
