using UnityEngine;

namespace CrossPlatform
{
    public class MobileInput : PlayerInput
    {
        [SerializeField] private Joystick m_joystick;
        [SerializeField] private PanelPlayerRotation m_panelRotation;
        [SerializeField] private PlayerJumpButton m_jumpButton;

        public override Vector2 Move => m_joystick.Direction;

        public override Vector2 RotationDelta => m_panelRotation.DeltaPosition;

        public override bool Jump => m_jumpButton.IsDown;
    }
}