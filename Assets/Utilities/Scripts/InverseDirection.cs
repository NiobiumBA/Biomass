using UnityEngine;

public class InverseDirection : MonoBehaviour
{
#if UNITY_EDITOR
    [ContextMenu("Inverse")]
    private void Inverse()
    {
        transform.forward = -transform.forward;
    }
#endif
}
