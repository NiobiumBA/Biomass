using UnityEngine;
using UnityEngine.EventSystems;

public class PanelPlayerRotation : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IUpdateSelectedHandler
{
    [SerializeField] private float m_sensitivity = 1;

    private bool m_isDragging;
    private PointerEventData m_currentEventData;

    public Vector2 DeltaPosition => m_isDragging ? m_currentEventData.delta * m_sensitivity : Vector2.zero;

    private void Start()
    {
        m_isDragging = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_isDragging = true;
        m_currentEventData = eventData;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_isDragging = false;
    }

    public void OnUpdateSelected(BaseEventData eventData)
    {
        m_isDragging = true;
        m_currentEventData = eventData as PointerEventData;
        print(true);
    }
}
