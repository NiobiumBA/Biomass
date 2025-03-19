using Localization;
using PlayerSettings;
using System;
using UnityEngine;

public class FractalSettingsCreator : MonoBehaviour
{
    [SerializeField] private BaseOption[] m_optionPrefabs;
    [SerializeField] private string m_translationId;

    public TOption CreateOption<TOption, TValue>(string name, string translationId, Transform layout)
        where TOption : Option<TValue>
    {
        foreach (BaseOption option in m_optionPrefabs)
        {
            if (option is TOption tOption)
            {
                TOption instance = Instantiate(tOption, layout);

                instance.OptionName = name;

                if (instance is TranslatedOption<TValue> translatedOption)
                    translatedOption.TranslationId = XmlTranslationParser.CombineNodesName(m_translationId, translationId);

                return instance;
            }
        }

        throw new Exception($"Does not found an option prefab with type {typeof(TOption)}");
    }

    private void ValidateOptionPrefabs()
    {
        if (m_optionPrefabs == null)
            return;

        for (int i = 0; i < m_optionPrefabs.Length - 1; i++)
        {
            BaseOption firstOption = m_optionPrefabs[i];

            for (int j = i + 1; j < m_optionPrefabs.Length; j++)
            {
                BaseOption secondOption = m_optionPrefabs[j];

                if (firstOption.GetType() == secondOption.GetType())
                {
                    Debug.LogWarning($"There are two prefabs in {nameof(DefaultSettingsCreator)} with one type." +
                        $" One of them will be ignored");
                }
            }
        }
    }
}
