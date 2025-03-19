using UnityEngine;

public class JoystickResize : MonoBehaviour
{
    public float Size;

    [SerializeField]
    private RectTransform m_joystick;
    [SerializeField]
    private Canvas m_canvas;

    private void Start()
    {
        Resize();
    }

    public void Resize()
    {
        float average = m_canvas.pixelRect.height + m_canvas.pixelRect.width;
        average *= 0.5f;

        m_joystick.localScale = Size * average * Vector3.one;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Resize();
    }

    private void Update()
    {
        Resize();
    }
#endif
}
