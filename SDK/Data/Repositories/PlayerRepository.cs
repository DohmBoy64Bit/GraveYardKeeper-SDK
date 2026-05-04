using GraveSDK.Data.Models;
using UnityEngine;

namespace GraveSDK.Data.Repositories
{
    public class PlayerRepository
    {
        public PlayerStats GetStats()
        {
            var save = MainGame.me?.save;
            var player = MainGame.me?.player;
            if (save == null || player == null) return null;

            return new PlayerStats
            {
                Health = save.GetHPPercentage() * save.max_hp,
                HealthMax = save.max_hp,
                Energy = player.energy,
                EnergyMax = save.max_energy,
                Sanity = player.sanity,
                SanityMax = save.max_sanity,
                Money = player.data.money,
                GratitudePoints = player.GetParam("gratitude_points", 0f),
                TechRed = Mathf.RoundToInt(player.GetParam("r", 0f)),
                TechGreen = Mathf.RoundToInt(player.GetParam("g", 0f)),
                TechBlue = Mathf.RoundToInt(player.GetParam("b", 0f))
            };
        }

        public void SetEnergy(float value)
        {
            if (MainGame.me?.player != null)
            {
                MainGame.me.player.energy = value;
            }
        }

        public void AddMoney(float amount)
        {
            if (MainGame.me?.player?.data != null)
            {
                MainGame.me.player.data.money += amount;
            }
        }

        public void AddTechPoints(int red, int green, int blue)
        {
            var player = MainGame.me?.player;
            if (player == null) return;

            if (red != 0) player.SetParam("r", player.GetParam("r", 0f) + red);
            if (green != 0) player.SetParam("g", player.GetParam("g", 0f) + green);
            if (blue != 0) player.SetParam("b", player.GetParam("b", 0f) + blue);
        }

        public void TeleportTo(Vector3 position)
        {
            if (MainGame.me?.player != null)
            {
                MainGame.me.player.transform.position = position;
            }
        }

        public Vector3 GetPosition()
        {
            return MainGame.me?.player?.transform.position ?? Vector3.zero;
        }
    }
}
