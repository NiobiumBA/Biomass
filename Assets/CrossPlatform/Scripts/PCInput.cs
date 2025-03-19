using UnityEngine;

namespace CrossPlatform
{
    public class PCInput : PlayerInput
    {
        private const string c_horizontalAxisName = "Horizontal";
        private const string c_verticalAxisName = "Vertical";
        private const string c_mouseXAxisName = "Mouse X";
        private const string c_mouseYAxisName = "Mouse Y";

        [SerializeField] private KeyCode m_jumpKey = KeyCode.Space;

        public override Vector2 Move
        {
            get
            {
                Vector2 move = new(Input.GetAxis(c_horizontalAxisName),
                                   Input.GetAxis(c_verticalAxisName));
                return Vector2.ClampMagnitude(move, 1f);
            }
        }

        public override Vector2 RotationDelta
        {
            get
            {
                return new Vector2(Input.GetAxis(c_mouseXAxisName),
                                   Input.GetAxis(c_mouseYAxisName));
            }
        }

        public override bool Jump => Input.GetKey(m_jumpKey);
    }
}