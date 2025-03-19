using RayMarching.Physics;
using RayMarching.Rendering;
using UnityEngine;

namespace RayMarching.Fractals
{
    public class FractalPresenter : SDFObjectPresenter
    {
        [SerializeField] private FractalAsset m_fractalAsset;

        public override IVisualSDFObject VisualObject => FractalObject;
        public override IPhysicsSDFObject PhysicsObject => FractalObject;

        public FractalAsset FractalAsset
        {
            get => m_fractalAsset;
            set
            {
                m_fractalAsset = value;

                UpdateActiveScene();
            }
        }

        private ISDFObject FractalObject
        {
            get
            {
                if (m_fractalAsset == null)
                    return null;

                return m_fractalAsset.SDFObject;
            }
        }
    }
}