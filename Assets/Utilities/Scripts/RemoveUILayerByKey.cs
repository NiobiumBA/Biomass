using UnityEngine;

public class RemoveUILayerByKey : MonoBehaviour
{
    [SerializeField] private KeyCode m_key = KeyCode.Escape;
    [SerializeField] private UILayers m_layers;

    private void Update()
    {
        if (Input.GetKeyDown(m_key))
        {
            if (m_layers.Layers.Count > 1)
                m_layers.RemoveLayer();
        }
    }
}
