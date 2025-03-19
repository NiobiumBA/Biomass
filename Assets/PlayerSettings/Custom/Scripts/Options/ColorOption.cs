using PlayerSettings;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ColorOption : TranslatedOption<Color>
{
    [SerializeField] private ColorSelector m_colorSelectingPrefab;
    [SerializeField] private Button m_button;
    [SerializeField] private RawImage m_colorPreview;

    private ColorSelector m_colorSelecting;

    public override Color Value { get => SelectingInstance.SelectedColor; set => SelectingInstance.SelectedColor = value; }
    
    private ColorSelector SelectingInstance
    {
        get
        {
            if (m_colorSelecting == null)
            {
                Transform parent = GetComponentInParent<Canvas>().transform;

                m_colorSelecting = Instantiate(m_colorSelectingPrefab, parent);
                m_colorSelecting.gameObject.SetActive(false);
                m_colorSelecting.OnColorChange += OnColorChange;

                UpdateColorPreview();
            }

            return m_colorSelecting;
        }
    }

    public override void Deserialize(byte[] bytes)
    {
        float red = BitConverter.ToSingle(bytes, 0);
        float green = BitConverter.ToSingle(bytes, sizeof(float));
        float blue = BitConverter.ToSingle(bytes, 2 * sizeof(float));
        float alpha = BitConverter.ToSingle(bytes, 3 * sizeof(float));

        Value = new Color(red, green, blue, alpha);
    }

    public override byte[] Serialize()
    {
        byte[] buffer = new byte[4 * sizeof(float)];
        
        using (MemoryStream stream = new(buffer))
        {
            stream.Write(BitConverter.GetBytes(Value.r));
            stream.Write(BitConverter.GetBytes(Value.g));
            stream.Write(BitConverter.GetBytes(Value.b));
            stream.Write(BitConverter.GetBytes(Value.a));
        }

        return buffer;
    }

    public void EnableColorSelecting()
    {
        SelectingInstance.gameObject.SetActive(true);
    }

    public void DisableColorSelecting()
    {
        SelectingInstance.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        m_button.onClick.AddListener(EnableColorSelecting);
    }

    private void OnDisable()
    {
        m_button.onClick?.RemoveListener(EnableColorSelecting);
    }

    private void OnColorChange()
    {
        UpdateColorPreview();

        OnValueChangeInvoke();
    }

    private void UpdateColorPreview()
    {
        m_colorPreview.color = Value;
    }
}
