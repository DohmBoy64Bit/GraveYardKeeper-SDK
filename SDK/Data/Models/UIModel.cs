using System.Collections.Generic;
using UnityEngine;

namespace GraveSDK.Data.Models
{
    public enum WindowType
    {
        Inventory,
        Craft,
        TechTree,
        Map,
        QuestList,
        Vendor,
        Chest,
        Journal,
        MainMenu,
        InGameMenu,
        Dialog
    }

    public class WindowState
    {
        public WindowType Type { get; set; }
        public bool IsOpen { get; set; }
        public bool IsFocused { get; set; }
    }
}
