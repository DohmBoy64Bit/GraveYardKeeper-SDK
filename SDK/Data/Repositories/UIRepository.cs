using System;
using System.Collections.Generic;
using GraveSDK.Data.Models;
using UnityEngine;

namespace GraveSDK.Data.Repositories
{
    public class UIRepository
    {
        public bool IsAnyWindowOpen => GUIElements.me?.IsAnyMassiveWindowOpened() ?? false;

        public bool IsWindowOpen(WindowType type)
        {
            var gui = GUIElements.me;
            if (gui == null) return false;

            switch (type)
            {
                case WindowType.Inventory: return gui.inventory.is_shown;
                case WindowType.Craft: return gui.craft.is_shown;
                case WindowType.TechTree: return gui.tech_tree.is_shown;
                case WindowType.Map: return gui.map.is_shown;
                case WindowType.QuestList: return gui.quest_list.gameObject.activeInHierarchy;
                case WindowType.Vendor: return gui.vendor.is_shown;
                case WindowType.Chest: return gui.chest.is_shown;
                case WindowType.MainMenu: return gui.main_menu.is_shown;
                case WindowType.InGameMenu: return gui.ingame_menu.is_shown;
                case WindowType.Dialog: return gui.dialog.is_shown;
                default: return false;
            }
        }

        public void CloseAllWindows()
        {
            GUIElements.me?.CloseAllInGameWindows();
        }

        public void ShowMessage(string message)
        {
            GUIElements.me?.dialog?.OpenOK(message, null, "", false, "");
        }

        public void ShowYesNo(string message, Action onYes, Action onNo = null)
        {
            GUIElements.me?.dialog?.OpenYesNo(message, () => onYes?.Invoke(), () => onNo?.Invoke(), null);
        }

        public void SetHUDVisible(bool visible, bool animated = true)
        {
            GUIElements.ChangeHUDAlpha(visible, animated);
        }

        public void ShowNotification(string text)
        {
            // text_window.Open(string txt_id, GJCommons.VoidDelegate on_closed = null)
            GUIElements.me?.text_window?.Open(text, null);
        }
    }
}
