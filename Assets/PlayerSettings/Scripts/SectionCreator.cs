using UnityEngine;

namespace PlayerSettings
{
    public abstract class SectionCreator : MonoBehaviour
    {
        public abstract TOption CreateOption<TOption, TValue>(string name)
            where TOption : Option<TValue>;
    }
}