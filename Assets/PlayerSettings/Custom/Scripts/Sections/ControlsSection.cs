using PlayerSettings;
using UnityEngine;

public class ControlsSection : Section
{
    [SerializeField] private bool m_canApply;
    [SerializeField] private PlayerMovement m_movement;
    [SerializeField] private string m_name;

    private SliderOption m_sensitivityOption;

    public override void Apply()
    {
        if (m_canApply == false)
            return;

        m_movement.Sensitivity = m_sensitivityOption.TransformedValue;
    }

    public override void Create()
    {
        m_sensitivityOption = CreateOption<SliderOption, int>("sensitivity");
        m_sensitivityOption.MinValue = 0.1f;
        m_sensitivityOption.MaxValue = 6;
        m_sensitivityOption.ValuesCount = 60;
        m_sensitivityOption.SetRoundTransformedValue(2);
    }
}
