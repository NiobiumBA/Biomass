using UnityEngine;

namespace PlayerSettings
{
    public class SettingsLoaderOnStart : MonoBehaviour
    {
        [SerializeField] private SettingsRepository m_repository;

        private void Start()
        {
            m_repository.Load();
            m_repository.Apply();
        }
    }
}