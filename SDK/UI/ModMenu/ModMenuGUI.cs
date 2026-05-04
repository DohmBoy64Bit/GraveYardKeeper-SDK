using UnityEngine;
using GraveSDK.Data.Repositories;
using GraveSDK.Data.Models;
using GraveSDK.Core;
using System.Collections.Generic;
using System.Linq;

namespace GraveSDK.UI.ModMenu
{
    public class ModMenuGUI : MonoBehaviour
    {
        private bool _isVisible = false;
        private Rect _windowRect = new Rect(100, 100, 800, 600);
        private Vector2 _scrollPos;
        private TabType _currentTab = TabType.Player;
        
        private string _itemSearch = "";
        private string _craftSearch = "";
        
        private ItemRepository _itemRepo = new ItemRepository();
        private CraftRepository _craftRepo = new CraftRepository();
        private PlayerRepository _playerRepo = new PlayerRepository();
        private EnvironmentRepository _envRepo = new EnvironmentRepository();
        
        private enum TabType
        {
            Player,
            Environment,
            Items,
            Crafts,
            Settings
        }
        
        void Update()
        {
            if (Input.GetKeyDown(ConfigManager.Current.ToggleMenuKey))
            {
                _isVisible = !_isVisible;
                if (_isVisible)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    ConfigManager.Save(); // Save config when closing menu
                }
            }

            if (ConfigManager.Current.InfiniteEnergy)
            {
                var stats = _playerRepo.GetStats();
                if (stats != null && stats.Energy < stats.EnergyMax)
                {
                    _playerRepo.SetEnergy(stats.EnergyMax);
                }
            }

            if (ConfigManager.Current.EnableGodMode)
            {
                var stats = _playerRepo.GetStats();
                if (stats != null && MainGame.me.player.hp < stats.HealthMax)
                {
                    MainGame.me.player.hp = stats.HealthMax;
                }
            }
        }
        
        void OnGUI()
        {
            if (!_isVisible) return;
            
            _windowRect = GUI.Window(999, _windowRect, DrawWindow, "Graveyard Keeper Mod Menu (SDK)");
        }
        
        void DrawWindow(int windowID)
        {
            GUILayout.BeginHorizontal();
            
            // Sidebar for tabs
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(150));
            foreach (TabType tab in System.Enum.GetValues(typeof(TabType)))
            {
                if (GUILayout.Toggle(_currentTab == tab, tab.ToString(), GUI.skin.button))
                {
                    _currentTab = tab;
                }
            }
            GUILayout.EndVertical();
            
            // Main content area
            GUILayout.BeginVertical(GUI.skin.box);
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            {
                switch (_currentTab)
                {
                    case TabType.Player: DrawPlayerTab(); break;
                    case TabType.Environment: DrawEnvironmentTab(); break;
                    case TabType.Items: DrawItemsTab(); break;
                    case TabType.Crafts: DrawCraftsTab(); break;
                    case TabType.Settings: DrawSettingsTab(); break;
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }
        
        void DrawPlayerTab()
        {
            var stats = _playerRepo.GetStats();
            if (stats == null)
            {
                GUILayout.Label("Player not found (start game first)");
                return;
            }

            GUILayout.Label("Character Stats", GUI.skin.box);
            GUILayout.Label($"Health: {stats.Health:F1} / {stats.HealthMax:F1}");
            GUILayout.Label($"Energy: {stats.Energy:F1} / {stats.EnergyMax:F1}");
            GUILayout.Label($"Money: {Trading.FormatMoney(stats.Money, true, true)}");
            
            GUILayout.Space(10);
            GUILayout.Label("Quick Actions", GUI.skin.box);
            if (GUILayout.Button("Refill Health & Energy"))
            {
                _playerRepo.SetEnergy(stats.EnergyMax);
                MainGame.me.player.hp = stats.HealthMax;
            }
            
            if (GUILayout.Button("Add 10 Gold"))
            {
                _playerRepo.AddMoney(100000); 
            }
            
            GUILayout.Space(10);
            GUILayout.Label("Tech Points", GUI.skin.box);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+10 Red")) _playerRepo.AddTechPoints(10, 0, 0);
            if (GUILayout.Button("+10 Green")) _playerRepo.AddTechPoints(0, 10, 0);
            if (GUILayout.Button("+10 Blue")) _playerRepo.AddTechPoints(0, 0, 10);
            GUILayout.EndHorizontal();
        }
        
        void DrawEnvironmentTab()
        {
            var state = _envRepo.GetState();
            if (state == null) return;
            
            GUILayout.Label("Time & Weather", GUI.skin.box);
            GUILayout.Label($"Day: {state.DayCount} ({state.CurrentDay})");
            GUILayout.Label($"Time: {state.TimeOfDayK:F2} ({(state.IsNight ? "Night" : "Day")})");
            
            if (GUILayout.Button("Skip to Morning")) _envRepo.SetTime(0.15f);
            if (GUILayout.Button("Skip to Night")) _envRepo.SetTime(0.85f);
            if (GUILayout.Button("Next Day")) _envRepo.SkipToNextDay();
            
            GUILayout.Space(10);
            GUILayout.Label("Weather", GUI.skin.box);
            GUILayout.Label($"Current: {(state.IsRainy ? "Rainy" : "Clear")}");
            if (GUILayout.Button("Toggle Rain")) _envRepo.SetWeatherRain(!state.IsRainy);
        }
        
        void DrawItemsTab()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(60));
            _itemSearch = GUILayout.TextField(_itemSearch);
            GUILayout.EndHorizontal();
            
            var items = _itemRepo.GetAllItems()
                .Where(i => string.IsNullOrEmpty(_itemSearch) || i.DisplayName.ToLower().Contains(_itemSearch.ToLower()))
                .Take(100);
                
            foreach (var item in items)
            {
                GUILayout.BeginHorizontal(GUI.skin.box);
                GUILayout.Label($"{item.DisplayName} ({item.Id})", GUILayout.ExpandWidth(true));
                if (GUILayout.Button("Give", GUILayout.Width(60)))
                {
                    MainGame.me.player.data.inventory.Add(new Item(item.Id, 1));
                }
                GUILayout.EndHorizontal();
            }
        }
        
        void DrawCraftsTab()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(60));
            _craftSearch = GUILayout.TextField(_craftSearch);
            GUILayout.EndHorizontal();
            
            var crafts = _craftRepo.GetAllCrafts()
                .Where(c => string.IsNullOrEmpty(_craftSearch) || c.DisplayName.ToLower().Contains(_craftSearch.ToLower()))
                .Take(100);
                
            foreach (var craft in crafts)
            {
                GUILayout.BeginHorizontal(GUI.skin.box);
                GUILayout.Label($"{craft.DisplayName} ({craft.Id})", GUILayout.ExpandWidth(true));
                if (craft.IsLocked)
                {
                    if (GUILayout.Button("Unlock", GUILayout.Width(60)))
                    {
                        // Logic to unlock craft
                        MainGame.me.save.unlocked_crafts.Add(craft.Id);
                    }
                }
                else
                {
                    GUILayout.Label("Unlocked", GUILayout.Width(60));
                }
                GUILayout.EndHorizontal();
            }
        }
        
        void DrawSettingsTab()
        {
            GUILayout.Label("Mod Configuration", GUI.skin.box);
            
            ConfigManager.Current.EnableGodMode = GUILayout.Toggle(ConfigManager.Current.EnableGodMode, "Enable God Mode");
            ConfigManager.Current.InfiniteEnergy = GUILayout.Toggle(ConfigManager.Current.InfiniteEnergy, "Infinite Energy");
            
            GUILayout.Space(10);
            GUILayout.Label($"Movement Speed: {ConfigManager.Current.MovementSpeedMultiplier:F1}x");
            ConfigManager.Current.MovementSpeedMultiplier = GUILayout.HorizontalSlider(ConfigManager.Current.MovementSpeedMultiplier, 0.5f, 5.0f);
            
            GUILayout.Space(10);
            GUILayout.Label("Toggle Key: " + ConfigManager.Current.ToggleMenuKey.ToString());
            
            if (GUILayout.Button("Save Config"))
            {
                ConfigManager.Save();
            }
        }
    }
}
