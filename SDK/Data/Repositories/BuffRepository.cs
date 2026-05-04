using System;
using System.Collections.Generic;
using System.Linq;
using GraveSDK.Data.Models;

namespace GraveSDK.Data.Repositories
{
    public class BuffRepository
    {
        private readonly Dictionary<string, BuffModel> _cache;

        public BuffRepository()
        {
            _cache = new Dictionary<string, BuffModel>();
        }

        public BuffModel GetBuff(string buffId)
        {
            if (string.IsNullOrEmpty(buffId)) return null;
            if (_cache.TryGetValue(buffId, out var cached)) return cached;

            BuffDefinition def = GameBalance.me?.GetData<BuffDefinition>(buffId);
            if (def == null) return null;

            var model = ConvertToModel(def);
            _cache[buffId] = model;
            return model;
        }

        public List<BuffModel> GetAllBuffs()
        {
            if (GameBalance.me?.buffs_data == null) return new List<BuffModel>();
            return GameBalance.me.buffs_data
                .Select(d => GetBuff(d.id))
                .Where(m => m != null)
                .ToList();
        }

        private BuffModel ConvertToModel(BuffDefinition def)
        {
            return new BuffModel
            {
                Id = def.id,
                DisplayName = def.GetLocalizedName(),
                Description = def.GetDescriptionIfExists(),
                Duration = def.length?.EvaluateFloat(null, null) ?? 0f,
                IsHidden = def.is_hidden,
                ResourceEffects = ConvertGameRes(def.res),
                CustomIcon = def.GetIconName(),
                TickPeriod = def.tick_period,
                DoNotShowTimer = def.do_not_show_timer,
                CraftQualityBonus = def.craft_q,
                OverlayType = (GraveSDK.Data.Models.BuffOverlayType)def.overlay_type
            };
        }

        private Dictionary<string, float> ConvertGameRes(GameRes res)
        {
            // Placeholder - actual GameRes conversion would need atom list processing
            return new Dictionary<string, float>();
        }

        public void ClearCache()
        {
            _cache.Clear();
        }
    }
}
