using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterPlanetGravity : PlanetGravity
{
    public enum GravityType
    {
        Planet,
        Down
    }

    public override Vector3 Velocity { get => m_velocity; set => m_velocity = value; }
    public Vector3 Acceleration { get => m_acceleration; set => m_acceleration = value; }

    public GravityType gravityType;

    //[SerializeField]
    //private float maxAngle = 60f;

    //[SerializeField]
    //private LayerMask m_layerMask;

    private CharacterController m_characterController;
    private Vector3 m_velocity;
    private Vector3 m_acceleration;

    private void Start()
    {
        m_characterController = GetComponent<CharacterController>();
        
        Init();
    }

    //private void FixedUpdate()
    //{

    //    //Require for override of PlanetGravity
    //}

    private void Update()
    {
        //ChangeState();
    }

    protected override void SetForce(Vector3 force)
    {
        m_acceleration = gravityType == GravityType.Planet ? force : Physics.gravity;
    }

    public override Vector3 GetGravityDirection()
    {
        return gravityType == GravityType.Planet ? base.GetGravityDirection() : Physics.gravity.normalized;
    }

    protected override void ChangeState()
    {
        base.ChangeState();

        Vector3 lastPos = transform.position;
        CollisionFlags flag = m_characterController.Move(m_velocity * Time.fixedDeltaTime);

        if (flag != CollisionFlags.None)
        {
            Vector3 offset = transform.position - lastPos;
            m_characterController.Move(-offset);

            StopGravity();
        }

        m_velocity += m_acceleration * Time.fixedDeltaTime;
    }

    //private void OnControllerColliderHit(ControllerColliderHit hit)
    //{
    //    StopGravity();

    //    Vector3 normal = hit.point - transform.position;
    //    Vector3 normVelocity = m_acceleration.normalized;

    //    //float dotProduct = Vector3.Dot(normal, normVelocity);
    //    //print(dotProduct);
    //    //float minCosAngle = Mathf.Cos(Mathf.PI / 4f);

    //    float angle = Vector3.Angle(normal, normVelocity);

    //    print(angle);

    //    if (angle < maxAngle)
    //    {
    //        StopGravity();
    //    }
    //}

    private void StopGravity()
    {
        //m_acceleration = Vector3.zero;
        m_velocity = Vector3.zero;
    }
}
