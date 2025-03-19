using UnityEngine;

namespace CrossPlatform
{
    public abstract class PlayerInput : MonoBehaviour
    {
        public abstract Vector2 Move { get; }

        public abstract Vector2 RotationDelta { get; }

        public abstract bool Jump { get; }
    }
}