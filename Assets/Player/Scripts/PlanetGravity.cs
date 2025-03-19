using UnityEngine;

public class PlanetGravity : MonoBehaviour
{
    public virtual Vector3 Velocity { get => m_rigidbody.velocity; set => m_rigidbody.velocity = value; }
    public Transform PlanetCenter { get => m_planetCenter; }
    public float GravityAcceleration { get => m_gravityAcceleration; }

    [SerializeField]
    private Transform m_planetCenter;

    private Rigidbody m_rigidbody;
    private float m_gravityAcceleration;

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        
        Init();
    }

    protected void Init()
    {
        m_gravityAcceleration = Physics.gravity.magnitude;
    }

    private void FixedUpdate()
    {
        ChangeState();
    }

    protected virtual void SetForce(Vector3 force)
    {
        m_rigidbody.AddForce(force, ForceMode.Impulse);
    }

    public virtual Vector3 GetGravityDirection()
    {
        Vector3 direction = m_planetCenter.position - transform.position;
        return direction.normalized;
    }

    protected virtual void ChangeState()
    {
        Vector3 direction = GetGravityDirection();

        Vector3 force = m_gravityAcceleration * direction;
        SetForce(force);
    }
}
