using System;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelector : MonoBehaviour
{
    public event Action OnColorChange;

    [SerializeField] private Slider m_hueSlider;
    [SerializeField] private SaturationValuePanel m_saturationValuePanel;
    [SerializeField] private Image m_cursor;
    [SerializeField] private RawImage m_panelImage;

    private Material m_panelMaterial;

    public Color SelectedColor
    {
        get => Color.HSVToRGB(m_hueSlider.value, m_saturationValuePanel.Saturation, m_saturationValuePanel.Value);
        set
        {
            Color.RGBToHSV(value, out float hue, out float saturation, out float colorValue);
            m_hueSlider.@value = hue;
            m_saturationValuePanel.Saturation = saturation;
            m_saturationValuePanel.Value = colorValue;

            HueUpdate();

            OnColorChange?.Invoke();
        }
    }

    public float Hue
    {
        get => m_hueSlider.@value;
        set => m_hueSlider.@value = Mathf.Clamp01(value);
    }

    public float Saturation
    {
        get => m_saturationValuePanel.Saturation;
        set => m_saturationValuePanel.Saturation = value;
    }

    public float Value
    {
        get => m_saturationValuePanel.Value;
        set => m_saturationValuePanel.Value = value;
    }

    private Material PanelMaterial
    {
        get
        {
            if (m_panelMaterial == null)
            {
                m_panelMaterial = new Material(m_panelImage.material);
                m_panelImage.material = m_panelMaterial;
            }

            return m_panelMaterial;
        }
    }

    private void OnEnable()
    {
        m_hueSlider.onValueChanged.AddListener(OnHueChange);
        m_saturationValuePanel.OnColorChange += OnSaturationValueChange;
    }

    private void OnDisable()
    {
        m_hueSlider.onValueChanged?.RemoveListener(OnHueChange);
        m_saturationValuePanel.OnColorChange -= OnSaturationValueChange;
    }

    private void OnHueChange(float arg0)
    {
        HueUpdate();

        OnColorChange?.Invoke();
    }

    private void HueUpdate()
    {
        float inverseHue = (0.5f + Hue) % 1;
        m_cursor.color = Color.HSVToRGB(inverseHue, 1, 1);

        PanelMaterial.SetFloat("_Hue", Hue);
    }

    private void OnSaturationValueChange()
    {
        OnColorChange?.Invoke();
    }
}
