using PlayerSettings;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FractalTypeChanger : MonoBehaviour
{
    [SerializeField] private Dropdown m_dropdown;
    [SerializeField] private ChildSwitcher m_sectionsSwitcher;
    [SerializeField] private FractalSectionInfo[] m_sectionInfos;

    private bool m_sectionsCreated = false;

    public FractalSectionInfo SelectedFractal => m_sectionInfos[m_dropdown.value];

    private IReadOnlyList<FractalSectionInfo> SectionInfos
    {
        get
        {
            if (m_sectionsCreated == false)
            {
                for (int i = 0; i < m_sectionInfos.Length; i++)
                {
                    Section section = m_sectionInfos[i].Section;
                    
                    section.Create();
                    section.OnSectionChanged += section.Apply;
                }

                m_sectionsSwitcher.SetChild(null);

                m_sectionsCreated = true;
            }

            return m_sectionInfos;
        }
    }

    private void OnEnable()
    {
        m_dropdown.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnDisable()
    {
        m_dropdown.onValueChanged?.RemoveListener(OnValueChanged);
    }

    private void Start()
    {
        List<Dropdown.OptionData> dropDownOptions = new();

        foreach (FractalSectionInfo info in SectionInfos)
        {
            Dropdown.OptionData optionData = new(info.SectionName);
            dropDownOptions.Add(optionData);
        }

        m_dropdown.options = dropDownOptions;

        SelectedFractal.Section.Apply();
        m_sectionsSwitcher.SetChild(SelectedFractal.Section.transform);
    }

    private void OnValueChanged(int value)
    {
        Section currentSection = m_sectionInfos[value].Section;
        m_sectionsSwitcher.SetChild(currentSection.transform);

        currentSection.Apply();
    }
}
