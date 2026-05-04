using System.Collections.Generic;

namespace GraveSDK.Data.Models
{
    public class TechModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string IconName { get; set; }
        public int BranchType { get; set; }
        public bool IsHidden { get; set; }
        public bool IsInvisible { get; set; }
        public TechStatus Status { get; set; }
        public List<string> ParentIds { get; set; }
        public List<string> UnlockIds { get; set; }
    }

    public enum TechStatus
    {
        Purchased,
        Unavailable,
        Available,
        Hidden,
        Invisible
    }
}
