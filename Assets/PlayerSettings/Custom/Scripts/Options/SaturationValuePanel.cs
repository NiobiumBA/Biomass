using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaturationValuePanel : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    public event Action OnColorChange;

    [SerializeField] private RectTransform m_cursor;

    public float Saturation
    {
        get => GetSaturation(m_cursor.position.x);
        set => SetSaturationAndValue(value, Value);
    }

    public float Value
    {
        get => GetValue(m_cursor.position.y);
        set => SetSaturationAndValue(Saturation, value);
    }

    public RectTransform Cursor => m_cursor;

    public void OnDrag(PointerEventData eventData)
    {
        UpdateColor(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UpdateColor(eventData);
    }

    private void UpdateColor(PointerEventData eventData)
    {
        float saturation = GetSaturation(eventData.position.x);
        float value = GetValue(eventData.position.y);

        SetSaturationAndValue(saturation, value);

        OnColorChange?.Invoke();
    }

    private void SetSaturationAndValue(float saturation, float value)
    {
        GetCorners(out Vector2 leftBottom, out Vector2 rightTop);

        m_cursor.position = new Vector3(Mathf.Lerp(leftBottom.x, rightTop.x, saturation),
                                        Mathf.Lerp(leftBottom.y, rightTop.y, value));
    }

    private float GetSaturation(float cursorX)
    {
        GetCorners(out Vector2 leftBottom, out Vector2 rightTop);

        return Mathf.InverseLerp(leftBottom.x, rightTop.x, cursorX);
    }

    private float GetValue(float cursorY)
    {
        GetCorners(out Vector2 leftBottom, out Vector2 rightTop);

        return Mathf.InverseLerp(leftBottom.y, rightTop.y, cursorY);
    }

    private void GetCorners(out Vector2 leftBottom, out Vector2 rightTop)
    {
        RectTransform rectTransform = transform as RectTransform;

        Rect rect = rectTransform.rect;

        leftBottom = rectTransform.TransformPoint(rect.min);
        rightTop = rectTransform.TransformPoint(rect.max);
    }
}
