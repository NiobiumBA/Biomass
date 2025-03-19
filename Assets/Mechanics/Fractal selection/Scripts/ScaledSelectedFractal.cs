using RayMarching;
using RayMarching.Physics;
using RayMarching.Rendering;
using RayMarching.SDFUtilities;
using UnityEngine;

public class ScaledSelectedFractal : SDFObjectPresenter
{
    [SerializeField] private float m_scale = 1;

    private ISDFObject m_scaledFractal;

    public ISDFObject ScaledFractal
    {
        get
        {
            if (m_scaledFractal == null && SelectedFractalContainer.Fractal != null)
                m_scaledFractal = new ScaleSDF(SelectedFractalContainer.Fractal, m_scale);

            return m_scaledFractal;
        }
    }

    public override IVisualSDFObject VisualObject => ScaledFractal;

    public override IPhysicsSDFObject PhysicsObject => ScaledFractal;
}
