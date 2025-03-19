using UnityEngine;

namespace CrossPlatform
{
    public class CrossPlatformInput : PlayerInput
    {
        [SerializeField] private MobileInput m_mobileInput;
        [SerializeField] private PCInput m_PCInput;

        private PlayerInput m_nativeInput;

        public PlayerInput NativeInput
        {
            get
            {
                if (m_nativeInput == null)
                {
                    m_nativeInput = Application.isMobilePlatform ? m_mobileInput : m_PCInput;
                }

                return m_nativeInput;
            }
        }

        public override Vector2 Move => NativeInput.Move;

        public override Vector2 RotationDelta => NativeInput.RotationDelta;

        public override bool Jump => NativeInput.Jump;
    }
}