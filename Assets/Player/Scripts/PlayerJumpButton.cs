using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerJumpButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    private bool m_isDown = false;

    public bool IsDown => m_isDown;

    public void OnPointerUp(PointerEventData eventData)
    {
        m_isDown = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_isDown = true;
    }
}
