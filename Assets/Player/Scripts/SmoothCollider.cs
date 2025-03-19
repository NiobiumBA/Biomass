using UnityEngine;

public class SmoothCollider : MonoBehaviour
{
    [SerializeField]
    private Transform m_colliderTransform;
    [SerializeField]
    private float m_smoothTime = 1f / 50f;
    private Vector3 m_velocity;

    private void Start()
    {
        //m_colliderTransform.position = transform.position;
        transform.position = m_colliderTransform.position;
    }

    private void Update()
    {
        //Vector3 newPos = Vector3.SmoothDamp(m_colliderTransform.position, transform.position, ref m_velocity, m_smoothTime, Time.deltaTime);

        //m_colliderTransform.SetPositionAndRotation(newPos, transform.rotation);

        Vector3 newPos = Vector3.SmoothDamp(transform.position, m_colliderTransform.position, ref m_velocity, m_smoothTime);

        transform.SetPositionAndRotation(newPos, m_colliderTransform.rotation);
    }

    private static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float deltaTime)
    {
        float velocityX = currentVelocity.x;
        float velocityY = currentVelocity.y;
        float velocityZ = currentVelocity.z;

        current.x = Mathf.SmoothDamp(current.x, target.x, ref velocityX, smoothTime, float.PositiveInfinity, deltaTime);
        current.y = Mathf.SmoothDamp(current.y, target.y, ref velocityY, smoothTime, float.PositiveInfinity, deltaTime);
        current.z = Mathf.SmoothDamp(current.z, target.z, ref velocityZ, smoothTime, float.PositiveInfinity, deltaTime);

        currentVelocity.Set(velocityX, velocityY, velocityZ);

        return current;
    }
}
