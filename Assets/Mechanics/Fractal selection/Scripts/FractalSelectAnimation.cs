using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FractalSelectAnimation : MonoBehaviour
{
    public enum SelectionState
    {
        None, AnimationPlaying, Enabled
    }

    [SerializeField] private Camera m_camera;
    [SerializeField] private float m_angleYDefault = -90;
    [SerializeField] private float m_angleYSelection = 0;
    [SerializeField] private float m_rotationSpeed = 1;
    [SerializeField] private AnimationCurve m_rotationCurve;
    [SerializeField] private UILayers m_layers;
    [SerializeField] private GameObject m_menuUI;
    [SerializeField] private GameObject m_fractalSelectingUI;
    [SerializeField] private int m_defaultRendererId;
    [SerializeField] private int m_rayMarchingRendererId;

    private SelectionState m_state = SelectionState.None;

    public SelectionState State => m_state;

    public void EnableSelection()
    {
        StartCoroutine(EnableSelectionCoroutine());
    }

    public void DisableSelection()
    {
        StartCoroutine(DisableSelectionCoroutine());
    }

    private IEnumerator EnableSelectionCoroutine()
    {
        if (m_state != SelectionState.None)
            yield break;

        m_state = SelectionState.AnimationPlaying;

        UniversalAdditionalCameraData cameraData = m_camera.GetUniversalAdditionalCameraData();
        cameraData.SetRenderer(m_rayMarchingRendererId);

        m_layers.RemoveLayer();

        Quaternion targetRotation = Quaternion.Euler(m_camera.transform.localEulerAngles.x,
                                                     m_angleYSelection,
                                                     m_camera.transform.localEulerAngles.z);
        yield return StartCoroutine(RotateCameraTo(targetRotation));

        m_layers.AddLayer(m_fractalSelectingUI);

        m_state = SelectionState.Enabled;
    }

    private IEnumerator DisableSelectionCoroutine()
    {
        if (m_state != SelectionState.Enabled)
            yield break;

        m_state = SelectionState.AnimationPlaying;

        m_layers.RemoveLayer();

        Quaternion targetRotation = Quaternion.Euler(m_camera.transform.localEulerAngles.x,
                                                    m_angleYDefault,
                                                    m_camera.transform.localEulerAngles.z);
        yield return StartCoroutine(RotateCameraTo(targetRotation));

        m_layers.AddLayer(m_menuUI);

        UniversalAdditionalCameraData cameraData = m_camera.GetUniversalAdditionalCameraData();
        cameraData.SetRenderer(m_defaultRendererId);

        m_state = SelectionState.None;
    }

    private IEnumerator RotateCameraTo(Quaternion targetRotation)
    {
        Quaternion startRotation = m_camera.transform.localRotation;
        float scaledTime = 0;

        while (scaledTime != 1)
        {
            float curveValue = m_rotationCurve.Evaluate(scaledTime);

            Quaternion currentRotation = Quaternion.Slerp(startRotation, targetRotation, curveValue);

            m_camera.transform.localRotation = currentRotation;

            scaledTime += Time.deltaTime * m_rotationSpeed;
            scaledTime = Mathf.Min(scaledTime, 1);

            yield return null;
        }
    }
}
