using RayMarching;
using RayMarching.Fractals;
using UnityEngine;

public class FractalChanger : MonoBehaviour
{
    [SerializeField] private FractalPresenter m_presenter;
    [SerializeField] private RandomizerFractal[] m_randomizers;

    private void Start()
    {
        RandomizerFractal randomizer = m_randomizers[Random.Range(0, m_randomizers.Length)];
        FractalAsset randomAsset = randomizer.RandomFractalAsset;

        m_presenter.FractalAsset = randomAsset;
    }
}
