using System;
using UnityEngine;
using Com.LuisPedroFonseca.ProCamera2D;

namespace GraveSDK.Data.Repositories
{
    public static class CameraRepository
    {
        private static ProCamera2DShake _shake;
        
        private static ProCamera2DShake ShakeComponent
        {
            get
            {
                if (_shake == null && MainGame.me != null)
                {
                    _shake = MainGame.me.GetComponent<ProCamera2DShake>() ?? MainGame.me.gameObject.AddComponent<ProCamera2DShake>();
                }
                return _shake;
            }
        }

        /// <summary>
        /// Triggers a screen shake effect.
        /// </summary>
        /// <param name="intensity">The strength of the shake.</param>
        /// <param name="duration">How long the shake lasts.</param>
        public static void Shake(float intensity = 1f, float duration = 0.5f)
        {
            var shake = ShakeComponent;
            if (shake != null)
            {
                shake.Shake(duration, new Vector2(intensity, intensity));
            }
        }

        /// <summary>
        /// Fades the screen to a color.
        /// </summary>
        public static void FadeOut(float duration = 1f, Color? color = null)
        {
            CameraTools.Fade(null, duration, color);
        }

        /// <summary>
        /// Fades the screen back from a color.
        /// </summary>
        public static void FadeIn(float duration = 1f, Color? color = null)
        {
            CameraTools.UnFade(null, duration, color);
        }

        /// <summary>
        /// Toggles a cinematic letterbox effect.
        /// </summary>
        public static void SetLetterbox(bool visible)
        {
            CameraTools.TweenLetterbox(visible);
        }

        /// <summary>
        /// Enables or disables a CameraFilterPack effect by name.
        /// Example: SetFilter("CameraFilterPack_TV_Old", true);
        /// </summary>
        public static void SetFilter(string filterName, bool enabled)
        {
            if (Camera.main == null) return;
            
            var component = Camera.main.GetComponent(filterName) as MonoBehaviour;
            if (component == null && enabled)
            {
                Type type = Type.GetType(filterName);
                if (type != null)
                {
                    component = Camera.main.gameObject.AddComponent(type) as MonoBehaviour;
                }
            }

            if (component != null)
            {
                component.enabled = enabled;
            }
        }
        
        /// <summary>
        /// Centers the camera on a specific object.
        /// </summary>
        public static void FocusOn(WorldGameObject obj, float duration = 0.7f)
        {
            if (obj == null) return;
            CameraTools.CameraFlyTo(obj.tf, null, duration);
        }

        /// <summary>
        /// Returns the camera focus to the player.
        /// </summary>
        public static void FocusOnPlayer(float duration = 0.7f)
        {
            CameraTools.CameraFlyBack(null, duration);
        }
    }
}
