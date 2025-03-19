using CrossPlatform;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController m_characterController;
    [SerializeField] private PlayerInput m_playerInput;
    [SerializeField] private Transform m_camera;
    [SerializeField] private Transform m_rotateTransform;
    [SerializeField] private OrbitRotator m_orbitRotator;
    [SerializeField] private bool m_canMove = true;
    [SerializeField] private bool m_canRotate = true;
    [SerializeField] private float m_speed = 1;
    [SerializeField] private float m_mouseSensitivity = 1f;

    private Vector3 m_eulerAngles;


    public float Speed
    {
        get => m_speed;
        set => m_speed = Mathf.Max(value, 0);
    }

    public float Sensitivity
    {
        get => m_mouseSensitivity;
        set => m_mouseSensitivity = Mathf.Max(value, 0);
    }

    public Vector3 EulerAngles { get => m_eulerAngles; set => m_eulerAngles = value; }
    public bool CanMove { get => m_canMove; set => m_canMove = value; }
    public bool CanRotate
    {
        get => m_canRotate;
        set
        {
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;

            m_canRotate = value;
        }
    }

    private void Start()
    {
        m_eulerAngles = m_rotateTransform.localEulerAngles;

        CanMove = m_canMove;
    }

    private void Update()
    {
        if (m_canMove)
            Move();
    }

    private void LateUpdate()
    {
        if (m_canRotate)
            Rotate();

        UpdatePlanetRotation();
    }

    private void Move()
    {
        Vector2 playerInputMove = m_playerInput.Move;

        Vector3 localDirection = m_speed * new Vector3(playerInputMove.x, 0, playerInputMove.y);
        Vector3 finalDirection = m_rotateTransform.TransformDirection(localDirection);

        m_characterController.Move(finalDirection * Time.deltaTime);
    }

    private void Rotate()
    {
        UpdateRotation();
        SetNewRotation();
    }

    private void UpdatePlanetRotation()
    {
        m_orbitRotator.Rotate(transform);
    }

    private void UpdateRotation()
    {
        Vector2 rotationDelta = m_playerInput.RotationDelta;

        m_eulerAngles += m_mouseSensitivity * new Vector3(-rotationDelta.y, rotationDelta.x);
        m_eulerAngles = new Vector3(Mathf.Clamp(m_eulerAngles.x, -90, 90),
                                    m_eulerAngles.y % 360,
                                    m_eulerAngles.z);
    }

    private void SetNewRotation()
    {
        Vector3 currentAngles = m_rotateTransform.localEulerAngles;
        m_rotateTransform.localEulerAngles = new Vector3(currentAngles.x, m_eulerAngles.y, currentAngles.z);

        Vector3 currentCameraAngles = m_camera.localEulerAngles;
        m_camera.localEulerAngles = new Vector3(m_eulerAngles.x, currentCameraAngles.y, currentCameraAngles.z);
    }
}
