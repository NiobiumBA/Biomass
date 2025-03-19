using RayMarching.Rendering;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace RayMarching.Fractals
{
    [CreateAssetMenu(menuName = "Custom/Fractals/PseudoKleinian")]
    public class PseudoKleinianAsset : FractalAsset
    {
        private class PseudoKleinianObject : FractalObject<PseudoKleinianAsset>
        {
            private readonly static string[] s_keywords = new string[]
            {
                "PSEUDOKLEINIAN"
            };

            public override IReadOnlyCollection<string> ShaderKeywords => s_keywords;

            public PseudoKleinianObject(PseudoKleinianAsset asset) : base(asset)
            {
            }

            public override void ApplyContext(ISDFRendererContext context)
            {
                context.SetInteger("_FractalIters", Asset.Iterations);

                context.SetVector("CSize", Asset.CSize);
                context.SetFloat("Size", Asset.Size);
                context.SetVector("C", Asset.C);

                context.SetFloat("_ColorDifference", Asset.ColorDifference);
                context.SetColor("_FractalColor1", Asset.FirstColor);
                context.SetColor("_FractalColor2", Asset.SecondColor);
            }

            public override float SDF(Vector3 position)
            {
                return math.max(PseudoKleinianSDF(position), SphereSDF(position, 1));
            }

            float SphereSDF(float3 p, float radius)
            {
                return math.length(p) - radius;
            }

            float PseudoKleinianSDF(float3 p)
            {
                float minD = float.MaxValue;
                float3 minXYZ = minD;

                p.yz = p.zy;

                float3 or = p;
                float3 ap = p + 1.0f;
                float DEfactor = 2;
                float4 orbitTrap = minD;

                for (int i = 0; i < Asset.Iterations; i++)
                {
                    int3 equals = (int3)(ap == p);
                    if (math.dot(equals, equals) != 0)
                        break;

                    ap = p;
                    p = 2.0f * math.clamp(p, -Asset.CSize, Asset.CSize) - p;

                    float r2 = math.dot(p, p);
                    orbitTrap = math.min(orbitTrap, math.abs(new float4(p, r2)));
                    float k = math.max(Asset.Size / r2, 1.0f);
                    p *= k;
                    DEfactor *= k;

                    p += (float3)Asset.C;
                    orbitTrap = math.min(orbitTrap, math.abs(new float4(p, math.dot(p, p))));
                    minD = math.min(minD, math.length(p - or));
                    minXYZ = math.min(minXYZ, math.abs(p - or));
                }
                
                float dist = 0.5f * math.abs(p.z) / DEfactor;
                //float dist = 0.5 * abs(p.z + 0.1) / DEfactor;
                return dist;
            }
        }

        [SerializeField] private float m_scale = 1;
        [SerializeField] private int m_iterations = 10;
        [SerializeField] private Vector3 m_cSize = new(1.0f, 1, 1.3f);
        [SerializeField] private float m_size = 1;
        [SerializeField] private Vector3 m_c = new(-0.62f, -0.015f, -0.025f);
        [SerializeField, Range(0, 1)] private float m_colorDifference = 1;
        [SerializeField] private Color m_firstColor;
        [SerializeField] private Color m_secondColor;

        private PseudoKleinianObject m_SDFObject;

        public override ISDFObject SDFObject
        {
            get
            {
                if (m_SDFObject == null)
                    m_SDFObject = new PseudoKleinianObject(this);

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

        public Color SecondColor { get => m_secondColor; set => m_secondColor = value; }
        public Color FirstColor { get => m_firstColor; set => m_firstColor = value; }
        public float ColorDifference { get => m_colorDifference; set => m_colorDifference = value; }
        public Vector3 CSize { get => m_cSize; set => m_cSize = value; }
        public float Size { get => m_size; set => m_size = value; }
        public Vector3 C { get => m_c; set => m_c = value; }

        private void OnValidate()
        {
            if (m_scale <= 0)
                m_scale = 1;

            if (m_iterations <= 0)
                m_iterations = 1;
        }
    }
}