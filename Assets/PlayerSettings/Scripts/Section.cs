using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerSettings
{
    public abstract class Section : MonoBehaviour
    {
        public event Action OnSectionChanged;
        
        [SerializeField] private SectionCreator m_creator;

        private List<ISerializableOption> m_options = new();

        public IReadOnlyCollection<ISerializableOption> Options => m_options;

        public SectionCreator Creator => m_creator;

        public abstract void Apply();

        public abstract void Create();

        protected TOption CreateOption<TOption, TValue>(string name)
            where TOption : Option<TValue>
        {
            foreach (ISerializableOption currentOption in m_options)
            {
                if (currentOption.OptionName == name)
                    throw new ArgumentException($"The option with name {name} has already created", nameof(name));
            }

            TOption instance = m_creator.CreateOption<TOption, TValue>(name);

            m_options.Add(instance);
            instance.OnValueChange += () => OnSectionChanged?.Invoke();

            return instance;
        }

        protected TOption CreateOption<TOption, TValue>(string name, TValue defaultValue)
            where TOption : Option<TValue>
        {
            TOption option = CreateOption<TOption, TValue>(name);
            option.Value = defaultValue;
            return option;
        }
    }
}