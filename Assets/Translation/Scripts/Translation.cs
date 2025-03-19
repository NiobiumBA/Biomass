using System;
using System.Collections.Generic;
using UnityEngine;

namespace Localization
{
    public static class Translation
    {
        public const string LanguagesPath = "Translations";

        public static event Action OnLanguageChanged;

        private static Language m_language;
        private static List<string> m_languages;

        public static IReadOnlyList<string> Languages
        {
            get
            {
                if (m_languages == null)
                {
                    TextAsset[] assets = Resources.LoadAll<TextAsset>(LanguagesPath);
                    
                    List<string> result = new();

                    foreach (TextAsset asset in assets)
                    {
                        result.Add(asset.name);
                        Resources.UnloadAsset(asset);
                    }

                    m_languages = result;
                }

                return m_languages;
            }
        }

        public static string LanguageName => m_language?.Name;

        public static Language Language
        {
            get => m_language;
            set
            {
                if (value == m_language) return;

                m_language = value;
                OnLanguageChanged?.Invoke();
            }
        }

        public static string Translate(string id)
        {
            if (Language != null)
            {
                return Language.Translate(id);
            }

            return "%" + id;
        }
    }
}
