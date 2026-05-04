using System.Collections.Generic;

namespace GraveSDK.Data.Models
{
    public class NPCModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Alias { get; set; }
        public string CustomHeadSprite { get; set; }
        public bool IsInNPCList { get; set; }
        public bool IsRelationVisible { get; set; }
        public List<string> Groups { get; set; }
    }
}
