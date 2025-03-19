using System;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerSettings
{
    public class SliderOption : TranslatedOption<int>
    {
        private static readonly int s_maxFloatDigits = 15;

        [SerializeField] private Slider m_sliderGUI;
        [SerializeField] private Text m_textGUI;
        [SerializeField] private int m_valuesCount;
        [SerializeField] private float m_minValue;
        [SerializeField] private float m_maxValue;
        [SerializeField] private int displayedDigitsCount = s_maxFloatDigits;

        public override int Value
        {
            get => (int)m_sliderGUI.value;
            set
            {
                if (value < 0 || value >= m_valuesCount)
                    throw new ArgumentOutOfRangeException();

                m_sliderGUI.@value = value;

                UpdateValueText();
            }
        }

        public int ValuesCount
        {
            get => m_valuesCount;
            set
            {
                if (value <= 1)
                    throw new ArgumentOutOfRangeException();

                m_valuesCount = value;

                UpdateSliderParams();
            }
        }

        public float MinValue { get => m_minValue; set => m_minValue = value; }

        public float MaxValue
        {
            get => m_maxValue;
            set
            {
                m_maxValue = Mathf.Max(m_minValue, value);
            }
        }

        public float TransformedValue => m_minValue + Step * Value;

        public float Step => (m_maxValue - m_minValue) / (m_valuesCount - 1);

        public int DisplayedDigitsCount
        {
            get => displayedDigitsCount;
            set
            {
                displayedDigitsCount = Mathf.Clamp(value, 0, s_maxFloatDigits);
            }
        }

        public void SetRoundTransformedValue(float transformedValue)
        {
            if (transformedValue < MinValue || transformedValue >= MaxValue)
                throw new ArgumentOutOfRangeException();

            float offset = transformedValue - MinValue;

            int rounded = Mathf.RoundToInt(offset / Step);

            Value = rounded;
        }

        public override void Deserialize(byte[] bytes)
        {
            Value = BitConverter.ToInt32(bytes, 0);
        }

        public override byte[] Serialize()
        {
            return BitConverter.GetBytes(Value);
        }

        private void OnEnable()
        {
            m_sliderGUI.onValueChanged.AddListener(OnSliderChanged);
        }

        private void OnDisable()
        {
            m_sliderGUI.onValueChanged?.RemoveListener(OnSliderChanged);
        }

        protected override void Start()
        {
            base.Start();

            UpdateSliderParams();
            UpdateValueText();
        }

        private void OnSliderChanged(float newValue)
        {
            UpdateValueText();

            OnValueChangeInvoke();
        }

        private void UpdateValueText()
        {
            float rounded = (float)Math.Round(TransformedValue, DisplayedDigitsCount);
            m_textGUI.text = rounded.ToString();
        }

        private void UpdateSliderParams()
        {
            m_sliderGUI.wholeNumbers = true;
            m_sliderGUI.minValue = 0;
            m_sliderGUI.maxValue = m_valuesCount - 1;
        }
    }
}