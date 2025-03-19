using CrossPlatform;
using UnityEngine;

[RequireComponent(typeof(CharacterPlanetGravity))]
public class PlayerJump : MonoBehaviour
{
    [SerializeField] private PlayerInput m_playerInput;
    [SerializeField] private float m_jumpHeight;

    private CharacterPlanetGravity m_characterGravity;
    private bool m_isJumping;

    public float JumpHeight { get => m_jumpHeight; set => m_jumpHeight = value; }

    private void Start()
    {
        m_characterGravity = GetComponent<CharacterPlanetGravity>();
        m_isJumping = true;
    }

    private void Update()
    {
        if (m_playerInput.Jump && m_isJumping == false)
        {
            m_isJumping = true;

            //print(m_isJumping);

            Jump();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        m_isJumping = false;

        //print(m_isJumping);
    }

    private void Jump()
    {
        Vector3 jumpDir = transform.position - m_characterGravity.PlanetCenter.position;
        jumpDir.Normalize();

        m_characterGravity.Velocity = GetJumpVelocity() * jumpDir;
    }

    private float GetJumpVelocity()
    {
        return Mathf.Sqrt(2f * m_characterGravity.GravityAcceleration * m_jumpHeight);
    }
}
