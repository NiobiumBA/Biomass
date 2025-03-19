using Localization;
using PlayerSettings;
using UnityEngine;

public class FractalSectionInfo : MonoBehaviour
{
    [SerializeField] private Section m_section;
    [SerializeField] private string m_nameTranslationId;

    public Section Section => m_section;

    public string SectionName => Translation.Translate(m_nameTranslationId);
}
