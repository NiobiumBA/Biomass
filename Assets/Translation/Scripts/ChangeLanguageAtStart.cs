using UnityEngine;

namespace Localization
{
    public class ChangeLanguageAtStart : MonoBehaviour
    {
        [SerializeField] private string m_language;

        private void Start()
        {
            Translation.Language = XmlTranslationParser.LoadLanguage(m_language);
        }
    }
}