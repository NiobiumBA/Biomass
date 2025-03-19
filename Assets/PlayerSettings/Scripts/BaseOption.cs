using System;
using UnityEngine;

namespace PlayerSettings
{
    public abstract class BaseOption : MonoBehaviour, ISerializableOption
    {
        public event Action OnValueChange;

        public abstract string OptionName { get; set; }

        public abstract void Deserialize(byte[] bytes);
        public abstract byte[] Serialize();

        protected void OnValueChangeInvoke()
        {
            OnValueChange?.Invoke();
        }
    }
}