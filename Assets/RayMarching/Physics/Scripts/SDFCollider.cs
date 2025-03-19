using UnityEngine;

namespace RayMarching.Physics
{
    public class SDFCollider : MonoBehaviour
    {
        [SerializeField] private Transform m_prefabPlaneCollider;
        [SerializeField] private Transform m_planeParent;

        private SphereCollider m_sphereCollider;
        private CharacterController m_characterController;
        private Transform m_plane;

        public static PhysicsSDFScene ActivePhysicsScene => SDFSceneManager.ActiveScene?.PhysicsPart;

        public float Radius
        {
            get
            {
                if (m_sphereCollider != null)
                    return m_sphereCollider.radius;
                else if (m_characterController != null)
                    return m_characterController.radius;
                else
                    throw new System.InvalidOperationException($"{gameObject} has not {typeof(SphereCollider)} or {typeof(CharacterController)}");
            }
            set
            {
                value = Mathf.Max(value, 0);

                if (m_sphereCollider != null)
                    m_sphereCollider.radius = value;
                else if (m_characterController != null)
                    m_characterController.radius = value;
                else
                    throw new System.InvalidOperationException($"{gameObject} has not {typeof(SphereCollider)} or {typeof(CharacterController)}");
            }
        }

        private void Start()
        {
            m_plane = Instantiate(m_prefabPlaneCollider, m_planeParent);

            m_sphereCollider = GetComponent<SphereCollider>();
            m_characterController = GetComponent<CharacterController>();
        }

        private void FixedUpdate()
        {
            //CheckGlobalScale();

            if (ActivePhysicsScene == null)
                return;

            Vector3 position = transform.position;

            float maxDistance = 3f * Radius;

            float normalDelta = 0.25f * Radius;
            Vector3 rayDir = -ActivePhysicsScene.GetNormal(position, normalDelta);

            Ray ray = new(position, rayDir);

            if (ActivePhysicsScene.RayMarch(ray, out RaycastHit hit, maxDistance, iterations: 20, minDistance: 0.125f * Radius, normalDelta))
            {
                Vector3 forward = rayDir;

                if (forward == Vector3.zero)
                    forward = Vector3.down;

                Vector3 targetPos = hit.point;
                Quaternion targetRotation = Quaternion.LookRotation(forward);

                m_plane.SetPositionAndRotation(targetPos, targetRotation);
            }
        }

        private void CheckGlobalScale()
        {
            Vector3 scale = transform.lossyScale;

            bool axisIsEqual = Mathf.Abs(scale.x - scale.y) < 0.01f && Mathf.Abs(scale.x - scale.z) < 0.01f;
            
            Debug.Log($"{name} : {Mathf.Abs(scale.x - scale.y)}, {Mathf.Abs(scale.x - scale.z)}");

            if (axisIsEqual == false)
            {
                //Debug.LogWarning($"The axis of the global scale of {name} is not equals");
            }
        }
    }
}