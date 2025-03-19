using PlayerSettings;
using RayMarching.Fractals;
using UnityEngine;

public class JuliaSection : FractalSection<JuliaSetAsset>
{
    private SliderOption m_powerOption;
    
    private SliderOption m_radiusOption;
    private SliderOption m_firstAngleOption;
    private SliderOption m_secondAngleOption;
    
    private ColorOption m_firstColorOption;
    private ColorOption m_secondColorOption;
    private SliderOption m_colorDifference;

    public override void Apply()
    {
        Asset.Power = m_powerOption.TransformedValue;

        Asset.AdditiveConstant = new JuliaSetAsset.HyperComplex(m_radiusOption.TransformedValue,
                                                                m_firstAngleOption.TransformedValue,
                                                                m_secondAngleOption.TransformedValue);
        
        Asset.FirstColor = m_firstColorOption.Value;
        Asset.SecondColor = m_secondColorOption.Value;
        Asset.ColorDifference = m_colorDifference.TransformedValue;

        base.Apply();
    }

    public override void Create()
    {
        m_powerOption = CreateOption<SliderOption, int>("power");
        m_powerOption.MinValue = 3;
        m_powerOption.MaxValue = 10;
        m_powerOption.ValuesCount = (int)m_powerOption.MaxValue - (int)m_powerOption.MinValue + 1;
        m_powerOption.Value = 0;

        m_radiusOption = CreateOption<SliderOption, int>("radius");
        m_radiusOption.MinValue = 0.15f;
        m_radiusOption.MaxValue = 0.95f;
        m_radiusOption.ValuesCount = 50;
        m_radiusOption.DisplayedDigitsCount = 2;

        m_firstAngleOption = CreateOption<SliderOption, int>("firstAngle");
        m_firstAngleOption.MinValue = 0;
        m_firstAngleOption.MaxValue = 2 * Mathf.PI;
        m_firstAngleOption.ValuesCount = 50;
        m_firstAngleOption.Value = 0;
        m_firstAngleOption.DisplayedDigitsCount = 2;

        m_secondAngleOption = CreateOption<SliderOption, int>("secondAngle");
        m_secondAngleOption.MinValue = 0;
        m_secondAngleOption.MaxValue = 2 * Mathf.PI;
        m_secondAngleOption.ValuesCount = 50;
        m_secondAngleOption.Value = 0;
        m_secondAngleOption.DisplayedDigitsCount = 2;

        m_firstColorOption = CreateOption<ColorOption, Color>("firstColor", Color.white);

        m_secondColorOption = CreateOption<ColorOption, Color>("secondColor", Color.black);

        m_colorDifference = CreateOption<SliderOption, int>("colorDifference");
        m_colorDifference.MinValue = 0;
        m_colorDifference.MaxValue = 1;
        m_colorDifference.ValuesCount = 51;
        m_colorDifference.DisplayedDigitsCount = 2;
    }
}
