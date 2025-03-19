using System;
using System.Collections.Generic;
using Localization;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerSettings
{
    public class SwitcherOption : TranslatedOption<int>
    {
        [SerializeField] private TextUITranslation m_valueNameTranslation;
        [SerializeField] private Text m_valueNameGUI;
        [SerializeField] private bool m_translateValueName = true;
        [SerializeField] private Button m_buttonGUI;
        [SerializeField] private List<string> m_valueNames;

        private int m_index = 0;

        public override int Value
        {
            get => m_index;
            set
            {
                if (m_index < 0 || m_index >= m_valueNames.Count)
                    m_index = 0;
                else
                    m_index = value;

                UpdateValueText();
            }
        }

        public string NamedValue => m_valueNames[m_index];

        public List<string> ValueNames
        {
            get => m_valueNames;
            set
            {
                m_valueNames = value;
                UpdateValueText();
            }
        }

        public bool TranslateValueName
        {
            get => m_translateValueName;
            set
            {
                m_translateValueName = value;
                m_valueNameTranslation.enabled = value;

                UpdateValueText();
            }
        }

        public override void Deserialize(byte[] bytes)
        {
            Value = BitConverter.ToInt32(bytes, 0);
        }

        public override byte[] Serialize()
        {
            return BitConverter.GetBytes(Value);
        }

        protected override void Start()
        {
            base.Start();

            Value = m_index;
            TranslateValueName = m_translateValueName;
        }

        private void OnEnable()
        {
            m_buttonGUI.onClick.AddListener(SwitchValue);
        }

        private void OnDisable()
        {
            m_buttonGUI.onClick?.RemoveListener(SwitchValue);
        }

        private void UpdateValueText()
        {
            if (m_translateValueName)
                m_valueNameTranslation.Id = NamedValue;
            else
                m_valueNameGUI.text = NamedValue;
        }

        private void SwitchValue()
        {
            if (Value < m_valueNames.Count - 1)
            {
                Value++;
            }
            else
            {
                Value = 0;
            }

            OnValueChangeInvoke();
        }
    }
}