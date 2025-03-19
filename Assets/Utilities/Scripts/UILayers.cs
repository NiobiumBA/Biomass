using System;
using System.Collections.Generic;
using UnityEngine;

public class UILayers : MonoBehaviour
{
    public event Action<GameObject> OnAddLayer;
    public event Action OnRemoveLayer;

    private List<GameObject> m_layers = new();

    public GameObject LastLayer
    {
        get
        {
            if (m_layers.Count == 0)
                return null;

            return m_layers[^1];
        }
    }

    public IReadOnlyList<GameObject> Layers => m_layers;

    public void AddLayer(GameObject layer)
    {
        if (layer == null)
            throw new ArgumentNullException(nameof(layer));

        if (LastLayer != null)
            LastLayer.SetActive(false);

        m_layers.Add(layer);

        LastLayer.SetActive(true);

        OnAddLayer?.Invoke(layer);
    }

    public void RemoveLayer()
    {
        if (m_layers.Count == 0)
            throw new InvalidOperationException("There are no layers to remove.");

        LastLayer.SetActive(false);

        m_layers.RemoveAt(m_layers.Count - 1);

        if (LastLayer != null)
            LastLayer.SetActive(true);

        OnRemoveLayer?.Invoke();
    }
}
