using UnityEngine;

public class MenuActivator : MonoBehaviour
{
    [SerializeField] private KeyCode m_key = KeyCode.Escape;
    [SerializeField] private PlayerMovement m_playerMovement;
    [SerializeField] private PlayerJump m_playerJump;
    [SerializeField] private UILayers m_layers;
    [SerializeField] private GameObject m_menuUI;
    [SerializeField] private GameObject m_backgroundLayer;

    private bool m_controllerEnable;

    public bool ControllerEnable
    {
        get => m_controllerEnable;
        set
        {
            m_controllerEnable = value;

            m_playerMovement.CanMove = value;
            m_playerMovement.CanRotate = value;
            m_playerJump.enabled = value;
        }
    }

    private void Start()
    {
        ControllerEnable = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(m_key))
        {
            int layersCount = m_layers.Layers.Count;

            if (layersCount == 1)
            {
                m_layers.AddLayer(m_menuUI);
                ControllerEnable = false;
            }
            else if (layersCount == 2)
            {
                m_layers.RemoveLayer();
                ControllerEnable = true;
            }
            else if (layersCount != 0)
            {
                m_layers.RemoveLayer();
            }
        }
    }

    private void OnEnable()
    {
        m_layers.OnAddLayer += OnAddLayer;
        m_layers.OnRemoveLayer += OnRemoveLayer;
    }

    private void OnDisable()
    {
        m_layers.OnAddLayer -= OnAddLayer;
        m_layers.OnRemoveLayer -= OnRemoveLayer;
    }

    private void OnAddLayer(GameObject obj)
    {
        if (obj == m_menuUI)
        {
            ControllerEnable = false;
        }
    }

    private void OnRemoveLayer()
    {
        if (m_layers.Layers.Count == 1)
            ControllerEnable = true;
    }
}
