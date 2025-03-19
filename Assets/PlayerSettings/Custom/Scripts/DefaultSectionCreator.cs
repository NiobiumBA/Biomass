using Localization;
using PlayerSettings;
using UnityEngine;
using UnityEngine.UI;

public class DefaultSectionCreator : SectionCreator
{
    [SerializeField] private DefaultSettingsCreator m_settingsCreator;
    [SerializeField] private Button m_buttonGUI;
    [SerializeField] private string m_translationId;

    private bool m_isCreated = false;
    private Transform m_layout;

    public override TOption CreateOption<TOption, TValue>(string name)
    {
        if (m_isCreated == false)
        {
            m_layout = m_settingsCreator.CreateSectionLayout();

            m_settingsCreator.LinkButtonWithLayout(m_layout, m_buttonGUI);

            m_isCreated = true;
        }

        string translationId = XmlTranslationParser.CombineNodesName(m_translationId, name);

        return m_settingsCreator.CreateOption<TOption, TValue>(name, translationId, m_layout);
    }
}
