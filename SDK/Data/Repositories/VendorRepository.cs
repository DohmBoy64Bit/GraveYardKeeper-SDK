using System.Collections.Generic;
using System.Linq;
using GraveSDK.Data.Models;

namespace GraveSDK.Data.Repositories
{
    public class VendorRepository
    {
        private readonly Dictionary<string, VendorModel> _cache;

        public VendorRepository()
        {
            _cache = new Dictionary<string, VendorModel>();
        }

        public VendorModel GetVendor(string vendorId)
        {
            if (string.IsNullOrEmpty(vendorId)) return null;
            if (_cache.TryGetValue(vendorId, out var cached)) return cached;

            VendorDefinition def = GameBalance.me?.GetData<VendorDefinition>(vendorId);
            if (def == null) return null;

            var model = ConvertToModel(def);
            _cache[vendorId] = model;
            return model;
        }

        public List<VendorModel> GetAllVendors()
        {
            if (GameBalance.me?.vendors_data == null) return new List<VendorModel>();
            return GameBalance.me.vendors_data
                .Select(d => GetVendor(d.id))
                .Where(m => m != null)
                .ToList();
        }

        private VendorModel ConvertToModel(VendorDefinition def)
        {
            return new VendorModel
            {
                Id = def.id,
                ProductTypes = def.GetProductTypes(),
                StartTier = def.start_tire,
                StartMoney = def.start_money,
                DailyMoneyIncome = def.daily_money_income,
                LevelUpCosts = def.levelup_costs?.Select(c => c.ToString()).ToList() ?? new List<string>()
            };
        }

        public void ClearCache()
        {
            _cache.Clear();
        }

        /// <summary>
        /// Injects an item into a vendor's trade list at a specific tier.
        /// </summary>
        public void AddItemToTrade(string vendorId, string itemId, int tier, int baseCount)
        {
            VendorDefinition def = GameBalance.me?.GetData<VendorDefinition>(vendorId);
            if (def == null) return;

            if (def.count_modificators == null) def.count_modificators = new List<VendorDefinition.CountModificator>();
            
            // Check if already exists
            if (def.count_modificators.Any(m => m.item_name == itemId)) return;

            def.count_modificators.Add(new VendorDefinition.CountModificator
            {
                item_name = itemId,
                tier = tier,
                base_count = baseCount
            });
            
            def.SortCountModificators();
        }

        /// <summary>
        /// Sets the current money of a vendor in the active save.
        /// </summary>
        public void SetMoney(string vendorId, float money)
        {
            if (WorldMap.objs == null) return;
            var vendor = WorldMap.objs.FirstOrDefault(o => o.vendor != null && o.vendor.id == vendorId)?.vendor;
            if (vendor != null)
            {
                vendor.cur_money = money;
            }
        }
    }
}
