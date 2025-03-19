using RayMarching.Rendering;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace RayMarching.Fractals
{
    [CreateAssetMenu(menuName = "Custom/Fractals/MengerSphere")]
    public class MengerSphereAsset : FractalAsset
    {
        private class MandelbrotObject : FractalObject<MengerSphereAsset>
        {
            private readonly static string[] s_keywords = new string[]
            {
                "MENGERSPHERE"
            };

            public override IReadOnlyCollection<string> ShaderKeywords => s_keywords;

            public MandelbrotObject(MengerSphereAsset asset) : base(asset)
            {
            }

            public override void ApplyContext(ISDFRendererContext context)
            {
                context.SetInteger("_FractalIters", Asset.Iterations);

                context.SetFloat("_SmoothRadius", Asset.SmoothRadius);
                context.SetFloat("_ScaleFactor", Asset.ScaleFactor);
                context.SetFloat("_MaxSize", Asset.MaxSize);
                context.SetFloat("_ModOffsetPos", Asset.ModOffsetPos);
                context.SetMatrix("_IterationTransform", Asset.IterationMatrix);

                context.SetFloat("_ColorDifference", Asset.ColorDifference);
                context.SetColor("_FractalColor1", Asset.FirstColor);
                context.SetColor("_FractalColor2", Asset.SecondColor);
            }

            public override float SDF(Vector3 position)
            {
                float resultDist = SphereSDF(position, Asset.MaxSize - Asset.SmoothRadius) - Asset.SmoothRadius;
                float currentScale = 1.0f;

                for (int i = 0; i < Asset.Iterations; i++)
                {
                    position = Asset.IterationMatrix * new float4(position, 1);

                    float size = Asset.MaxSize * Asset.ModOffsetPos / currentScale;

                    position = Repeat(position, size);

                    currentScale *= Asset.ScaleFactor;
                    float3 r = position * currentScale;
                    float currentDist = (CylinderCrossSDF(r, Asset.MaxSize - Asset.SmoothRadius / currentScale) - Asset.SmoothRadius) / currentScale;

                    resultDist = Mathf.Max(resultDist, -currentDist);
                }

                return resultDist;
            }

            private float SphereSDF(Vector3 pos, float radius)
            {
                return pos.magnitude - radius;
            }

            private float CylinderSDF(Vector2 p, float size)
            {
                return p.magnitude - size;
            }

            private float CylinderCrossSDF(float3 p, float size)
            {
                float dxy = CylinderSDF(p.xy, size);
                float dyz = CylinderSDF(p.yz, size);
                float dzx = CylinderSDF(p.zx, size);
                return math.min(dxy, math.min(dyz, dzx));
            }

            private float3 Repeat(float3 p, float3 size)
            {
                float3 halfSize = size * 0.5f;
                p = math.fmod(p + halfSize, size) - halfSize;
                p = math.fmod(p - halfSize, size) + halfSize;
                return p;
            }
        }

        [SerializeField] private float m_scale = 1;
        [SerializeField] private int m_iterations = 10;
        [SerializeField] private float m_smoothRadius = 0.1f;
        [SerializeField] private float m_scaleFactor = 0.1f;
        [SerializeField] private float m_maxSize = 1;
        [SerializeField] private float m_modOffsetPos = 1;
        [SerializeField] private Vector3 m_iterationOffset;
        [SerializeField] private Vector3 m_iterationRotation;
        [SerializeField, Range(0, 1)] private float m_colorDifference = 1;
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

        public Matrix4x4 IterationMatrix
        {
            get
            {
                Quaternion rotation = Quaternion.Euler(IterationRotation);
                return Matrix4x4.TRS(IterationOffset, rotation, Vector3.one);
            }
        }

        public Color SecondColor { get => m_secondColor; set => m_secondColor = value; }
        public Color FirstColor { get => m_firstColor; set => m_firstColor = value; }
        public float ColorDifference { get => m_colorDifference; set => m_colorDifference = value; }
        public float SmoothRadius { get => m_smoothRadius; set => m_smoothRadius = value; }
        public float ScaleFactor { get => m_scaleFactor; set => m_scaleFactor = value; }
        public float MaxSize { get => m_maxSize; set => m_maxSize = value; }
        public float ModOffsetPos { get => m_modOffsetPos; set => m_modOffsetPos = value; }
        public Vector3 IterationOffset { get => m_iterationOffset; set => m_iterationOffset = value; }
        public Vector3 IterationRotation { get => m_iterationRotation; set => m_iterationRotation = value; }

        private void OnValidate()
        {
            if (m_scale <= 0)
                m_scale = 1;

            if (m_iterations <= 0)
                m_iterations = 1;
        }
    }
}