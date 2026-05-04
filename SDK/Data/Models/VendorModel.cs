using System.Collections.Generic;

namespace GraveSDK.Data.Models
{
    public class VendorModel
    {
        public string Id { get; set; }
        public List<string> ProductTypes { get; set; }
        public int StartTier { get; set; }
        public float StartMoney { get; set; }
        public float DailyMoneyIncome { get; set; }
        public List<string> LevelUpCosts { get; set; }
    }
}
