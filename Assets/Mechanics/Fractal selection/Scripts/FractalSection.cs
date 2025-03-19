using PlayerSettings;
using RayMarching.Fractals;
using UnityEngine;

public abstract class FractalSection<TAsset> : Section
    where TAsset : FractalAsset
{
    [SerializeField] private FractalPresenter m_presenter;
    [SerializeField] private TAsset m_sourceAsset;

    private TAsset m_asset;

    public TAsset Asset
    {
        get
        {
            if (m_asset == null)
            {
                //m_asset = ScriptableObject.CreateInstance<TAsset>();
                m_asset = Instantiate(m_sourceAsset);
            }

            return m_asset;
        }
    }

    public override void Apply()
    {
        SelectedFractalContainer.Fractal = Asset.SDFObject;
        m_presenter.FractalAsset = m_asset;
    }
}
