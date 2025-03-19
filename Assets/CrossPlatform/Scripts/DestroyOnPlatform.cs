using UnityEngine;

namespace CrossPlatform
{
    public class DestroyOnPlatform : MonoBehaviour
    {
        public enum Platform
        {
            PC, Mobile
        }

        [SerializeField] private Platform m_destroyOn;

        private Platform CurrentPlatform => Application.isMobilePlatform ? Platform.Mobile : Platform.PC;

        private void Start()
        {
            if (CurrentPlatform == m_destroyOn)
                Destroy(gameObject);
        }
    }
}