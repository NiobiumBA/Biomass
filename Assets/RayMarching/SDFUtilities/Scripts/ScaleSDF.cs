using RayMarching.Rendering;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RayMarching.SDFUtilities
{
    public class ScaleSDF : ISDFObject
    {
        public const string ScaleShaderKeyword = "SCALEFRACTAL";

        private readonly ISDFObject m_source;
        
        private float m_scale;
        private float m_inverseScale;

        private IReadOnlyCollection<string> m_lastSourceShaderKeywords;
        private List<string> m_lastShaderKeywords;

        public float Scale
        {
            get => m_scale;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "should be positive");

                m_scale = value;
                m_inverseScale = 1f / m_scale;
            }
        }

        public IReadOnlyCollection<string> ShaderKeywords
        {
            get
            {
                IReadOnlyCollection<string> sourceShaderKeywords = m_source.ShaderKeywords;

                if (m_lastShaderKeywords == null || sourceShaderKeywords != m_lastSourceShaderKeywords)
                {
                    m_lastShaderKeywords = new List<string>
                    {
                        ScaleShaderKeyword
                    };
                    
                    if (sourceShaderKeywords != null)
                        m_lastShaderKeywords.AddRange(sourceShaderKeywords);

                    m_lastSourceShaderKeywords = sourceShaderKeywords;
                }

                return m_lastShaderKeywords;
            }
        }

        public ScaleSDF(ISDFObject source, float scale)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            m_source = source;

            Scale = scale;
        }

        public float SDF(Vector3 position)
        {
            return m_source.SDF(position * m_inverseScale) * m_scale;
        }

        public void ApplyContext(ISDFRendererContext context)
        {
            context.SetFloat("_FractalScale", m_scale);
            context.SetFloat("_FractalInvScale", m_inverseScale);

            m_source.ApplyContext(context);
        }
    }
}