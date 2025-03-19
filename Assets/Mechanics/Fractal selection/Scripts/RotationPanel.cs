using UnityEngine;
using UnityEngine.EventSystems;

public class RotationPanel : MonoBehaviour, IDragHandler
{
    [SerializeField] private Canvas m_canvas;
    [SerializeField] private Transform m_orbit;
    [SerializeField] private float m_sensitivity;

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 deltaRotation = new Vector2(-eventData.delta.y, eventData.delta.x) * m_sensitivity;

        deltaRotation = new Vector2(deltaRotation.x / m_canvas.renderingDisplaySize.x,
                                    deltaRotation.y / m_canvas.renderingDisplaySize.y);

        m_orbit.localRotation *= Quaternion.Euler(deltaRotation);
    }
}
