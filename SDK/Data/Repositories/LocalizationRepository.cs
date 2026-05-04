using System;
using UnityEngine;

namespace GraveSDK.Data.Repositories
{
    public class LocalizationRepository
    {
        /// <summary>
        /// Gets the localized text for a given key.
        /// </summary>
        /// <param name="key">The localization key.</param>
        /// <param name="fallback">The fallback string if the key is not found.</param>
        /// <returns>Localized string or fallback.</returns>
        public string GetText(string key, string fallback = null)
        {
            if (string.IsNullOrEmpty(key)) return fallback ?? string.Empty;
            
            string result = Localization.Get(key, false);
            // In some versions of this localization engine, it returns the key if not found
            return result == key ? (fallback ?? key) : result;
        }

        /// <summary>
        /// Sets the current game language.
        /// </summary>
        /// <param name="languageName">The language name (e.g., "English", "Russian").</param>
        public void SetLanguage(string languageName)
        {
            Localization.language = languageName;
        }

        /// <summary>
        /// Gets all languages supported by the game.
        /// </summary>
        /// <returns>Array of language names.</returns>
        public string[] GetAvailableLanguages()
        {
            return Localization.knownLanguages;
        }

        /// <summary>
        /// Gets the currently active language.
        /// </summary>
        public string CurrentLanguage => Localization.language;

        /// <summary>
        /// Manually injects a localization key-value pair for the current language.
        /// Useful for mod-specific strings.
        /// </summary>
        public void AddCustomLocalization(string key, string text)
        {
            Localization.Set(Localization.language, key, text);
        }
    }
}
