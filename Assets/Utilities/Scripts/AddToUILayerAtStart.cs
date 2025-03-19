using UnityEngine;

public class AddToUILayerAtStart : MonoBehaviour
{
    [SerializeField] private UILayers m_layers;

    private void Start()
    {
        m_layers.AddLayer(gameObject);
    }
}
