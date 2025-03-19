using PlayerSettings;
using RayMarching.Fractals;
using UnityEngine;

public class MandelbrotSection : FractalSection<MandelbrotAsset>
{
    private SliderOption m_powerOption;

    private ColorOption m_firstColorOption;
    private ColorOption m_secondColorOption;
    private SliderOption m_colorDifference;

    public override void Apply()
    {
        Asset.Power = m_powerOption.TransformedValue;
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

        m_firstColorOption = CreateOption<ColorOption, Color>("firstColor", Color.white);

        m_secondColorOption = CreateOption<ColorOption, Color>("secondColor", Color.black);

        m_colorDifference = CreateOption<SliderOption, int>("colorDifference");
        m_colorDifference.MinValue = 0;
        m_colorDifference.MaxValue = 1;
        m_colorDifference.ValuesCount = 51;
        m_colorDifference.DisplayedDigitsCount = 2;
    }
}
