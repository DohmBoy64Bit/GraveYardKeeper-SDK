using System.Collections.Generic;

namespace GraveSDK.Data.Models
{
    public class ObjectModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public ObjectType Type { get; set; }
        public float MaxHp { get; set; }
        public int InventorySize { get; set; }
        public bool IsNPC { get; set; }
        public bool IsMob { get; set; }
        public string CustomIcon { get; set; }
        public List<string> Groups { get; set; }
    }

    public enum ObjectType
    {
        Default,
        Mob,
        NPC,
        PorterStation,
        SoulTotem
    }
}
