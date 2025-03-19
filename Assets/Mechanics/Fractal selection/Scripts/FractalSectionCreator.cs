using Localization;
using PlayerSettings;
using UnityEngine;

public class FractalSectionCreator : SectionCreator
{
    [SerializeField] private FractalSettingsCreator m_settingsCreator;
    [SerializeField] private string m_translationId;

    public override TOption CreateOption<TOption, TValue>(string name)
    {
        string translationId = XmlTranslationParser.CombineNodesName(m_translationId, name);

        return m_settingsCreator.CreateOption<TOption, TValue>(name, translationId, transform);
    }
}
