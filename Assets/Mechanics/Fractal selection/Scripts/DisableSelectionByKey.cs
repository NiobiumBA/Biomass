using UnityEngine;

public class DisableSelectionByKey : MonoBehaviour
{
    [SerializeField] private FractalSelectAnimation m_selection;
    [SerializeField] private KeyCode m_key = KeyCode.Escape;

    private void Update()
    {
        if (Input.GetKeyDown(m_key))
        {
            m_selection.DisableSelection();
        }
    }
}
