using PlayerSettings;
using RayMarching.Rendering;
using UnityEngine;

public class GraphicsSection : Section
{
    [SerializeField] private RayMarchingFeature m_rayMarching;
    [SerializeField] private RayMarchingShadows m_shadowsFeature;

    private SliderOption m_depthResolutionOption;
    private ToggleOption m_shadowsToggleOption;
    private SliderOption m_shadowResolutionOption;

    public override void Apply()
    {
        m_rayMarching.DepthResolution = GetResolution(m_depthResolutionOption);
        m_shadowsFeature.SetActive(m_shadowsToggleOption.Value);
        m_shadowsFeature.ShadowsResolution = GetResolution(m_shadowResolutionOption);
    }

    public override void Create()
    {
        m_depthResolutionOption = CreateSlider("depthResolution");
        bool defaultShadowsEnable = Application.isMobilePlatform == false;
        m_shadowsToggleOption = CreateOption<ToggleOption, bool>("shadowsEnable", defaultShadowsEnable);
        m_shadowResolutionOption = CreateSlider("shadowsResolution");
    }

    private Vector2Int GetResolution(SliderOption sliderOption)
    {
        Vector2Int minResolution = new(64, 32);
        int powerOf2 = 1 << (int)(sliderOption.TransformedValue - 1);
        Vector2Int depthResolution = minResolution * powerOf2;
        return depthResolution;
    }

    private SliderOption CreateSlider(string name)
    {
        SliderOption sliderOption = CreateOption<SliderOption, int>(name);
        sliderOption.MinValue = 1;
        sliderOption.ValuesCount = 7;
        sliderOption.MaxValue = 7;
        sliderOption.SetRoundTransformedValue(1);
        return sliderOption;
    }
}
