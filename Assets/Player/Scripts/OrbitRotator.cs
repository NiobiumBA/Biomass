using UnityEngine;

public class OrbitRotator : MonoBehaviour
{
    [SerializeField] private Transform m_planet;

    public void Rotate(Transform body)
    {
        Vector3 targetDir = body.position - m_planet.position;
        targetDir.Normalize();

        body.rotation = Quaternion.FromToRotation(body.up, targetDir) * body.rotation;
    }
}
