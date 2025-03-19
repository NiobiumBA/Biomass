using UnityEngine;
using UnityEngine.UI;

namespace Localization
{
    [RequireComponent(typeof(Text))]
    public class TextUITranslation : MonoBehaviour
    {
        [SerializeField] private string m_id;

        private Text m_textUI;

        public string Id
        {
            get => m_id;
            set
            {
                m_id = value;

                UpdateText();
            }
        }

        public Text TextUI
        {
            get
            {
                if (m_textUI == null)
                    m_textUI = GetComponent<Text>();

                return m_textUI;
            }
        }

        private void OnEnable()
        {
            UpdateText();

            Translation.OnLanguageChanged += UpdateText;
        }

        private void OnDisable()
        {
            Translation.OnLanguageChanged -= UpdateText;
        }

        private void UpdateText()
        {
            TextUI.text = Translation.Translate(m_id);
        }
    }
}