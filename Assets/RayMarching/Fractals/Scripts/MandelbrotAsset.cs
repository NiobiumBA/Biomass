using RayMarching.Rendering;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RayMarching.Fractals
{
    [CreateAssetMenu(menuName = "Custom/Fractals/Mandelbrot")]
    public class MandelbrotAsset : FractalAsset
    {
        private class MandelbrotObject : FractalObject<MandelbrotAsset>
        {
            private readonly static string[] s_keywords = new string[]
            {
                "MANDELBROT"
            };

            public override IReadOnlyCollection<string> ShaderKeywords => s_keywords;

            public MandelbrotObject(MandelbrotAsset asset) : base(asset)
            {
            }

            public override void ApplyContext(ISDFRendererContext context)
            {
                context.SetInteger("_FractalIters", Asset.Iterations);
                context.SetFloat("_FractalPower", Asset.Power);
                context.SetFloat("_ColorDifference", Asset.ColorDifference);
                context.SetColor("_FractalColor1", Asset.FirstColor);
                context.SetColor("_FractalColor2", Asset.SecondColor);
            }

            public override float SDF(Vector3 position)
            {
                Vector3 zn = position;
                float dr = 1.25f;
                float r = 1f, teta, phi;
                float rad_pow;
                float cos_phi;

                for (int i = 0; i < Asset.Iterations; i++)
                {
                    r = zn.magnitude;

                    if (r >= 2.0f)
                        break;

                    // convert to spherical coordinates
                    teta = Mathf.Atan2(zn.y, zn.x);
                    phi = Mathf.Asin(zn.z / r);

                    dr = Mathf.Pow(r, Asset.Power - 1.0f) * Asset.Power * dr + 1.0f;

                    rad_pow = Mathf.Pow(r, Asset.Power);
                    teta *= Asset.Power;
                    phi *= Asset.Power;

                    // convert back to cartesian coordinates
                    cos_phi = Mathf.Cos(phi);
                    zn = rad_pow * new Vector3(Mathf.Cos(teta) * cos_phi,
                                               Mathf.Sin(teta) * cos_phi,
                                               Mathf.Sin(phi));
                    zn += position;
                }

                return 0.5f * Mathf.Log(r) * r / dr;
            }
        }

        [SerializeField] private float m_scale = 1;
        [SerializeField] private int m_iterations = 10;
        [SerializeField] private float m_power = 2;
        [Range(0, 1), SerializeField] private float m_colorDifference = 1;
        [SerializeField] private Color m_firstColor;
        [SerializeField] private Color m_secondColor;

        private MandelbrotObject m_SDFObject;

        public override ISDFObject SDFObject
        {
            get
            {
                if (m_SDFObject == null)
                    m_SDFObject = new MandelbrotObject(this);

                return m_SDFObject;
            }
        }

        public override float Scale
        {
            get => m_scale;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "value must be positive");

                m_scale = value;
            }
        }

        public int Iterations
        {
            get => m_iterations;
            set
            {
                if (m_iterations <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "value must be positive");

                m_iterations = value;
            }
        }

        public float Power { get => m_power; set => m_power = value; }
        public Color SecondColor { get => m_secondColor; set => m_secondColor = value; }
        public Color FirstColor { get => m_firstColor; set => m_firstColor = value; }
        public float ColorDifference { get => m_colorDifference; set => m_colorDifference = value; }

        private void OnValidate()
        {
            if (m_scale <= 0)
                m_scale = 1;

            if (m_iterations <= 0)
                m_iterations = 1;
        }
    }
}