using Localization;
using UnityEngine;

namespace PlayerSettings
{
    public abstract class TranslatedOption<T> : Option<T>
    {
        [SerializeField] private TextUITranslation m_textTranslation;
        [SerializeField] private bool m_isTranslate = true;

        private string m_name;
        private string m_translationId;

        public override string OptionName
        {
            get => m_name;
            set
            {
                m_name = value;
                UpdateText();
            }
        }

        public virtual string TranslationId
        {
            get => m_translationId;
            set
            {
                m_translationId = value;
                UpdateText();
            }
        }

        public bool IsTranslate
        {
            get => m_isTranslate;
            set
            {
                m_isTranslate = value;
                UpdateText();
            }
        }

        protected virtual void Start()
        {
            UpdateText();
        }

        private void UpdateText()
        {
            m_textTranslation.enabled = IsTranslate;

            if (IsTranslate)
                m_textTranslation.Id = TranslationId == null ? string.Empty : TranslationId;
            else
                m_textTranslation.TextUI.text = OptionName;
        }
    }
}