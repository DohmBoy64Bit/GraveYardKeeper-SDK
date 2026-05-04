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
    }
}
