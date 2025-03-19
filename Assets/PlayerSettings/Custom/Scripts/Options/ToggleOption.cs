using System;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerSettings
{
    public class ToggleOption : TranslatedOption<bool>
    {
        [SerializeField] private Toggle m_toggleGUI;

        public override bool Value { get => m_toggleGUI.isOn; set => m_toggleGUI.isOn = value; }

        public override void Deserialize(byte[] bytes)
        {
            Value = BitConverter.ToBoolean(bytes, 0);
        }

        public override byte[] Serialize()
        {
            return BitConverter.GetBytes(Value);
        }

        private void OnEnable()
        {
            m_toggleGUI.onValueChanged.AddListener(OnValueChangedCall);
        }

        private void OnDisable()
        {
            m_toggleGUI.onValueChanged?.RemoveListener(OnValueChangedCall);
        }

        private void OnValueChangedCall(bool arg0)
        {
            OnValueChangeInvoke();
        }
    }
}