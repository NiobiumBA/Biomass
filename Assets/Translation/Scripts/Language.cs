using System;
using System.Collections.Generic;

namespace Localization
{
    public class Language
    {
        private readonly string m_name;
        private readonly Dictionary<string, string> m_translations;

        public string Name => m_name;

        public IReadOnlyDictionary<string, string> Translations => m_translations;

        public Language(string name, IReadOnlyDictionary<string, string> translations)
        {
            m_name = name;
            m_translations = new Dictionary<string, string>(translations);
        }

        public string Translate(string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (Translations.TryGetValue(id, out string result))
            {
                return result;
            }

            return "$" + id;
        }

        public override string ToString()
        {
            return m_name;
        }
    }
}