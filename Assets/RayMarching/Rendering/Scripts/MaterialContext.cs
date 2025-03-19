using System;
using System.Collections.Generic;
using UnityEngine;

namespace RayMarching.Rendering
{
    public class MaterialContext : ISDFRendererContext
    {
        private readonly Material m_material;

        public Material Material => m_material;

        public IReadOnlyCollection<string> EnabledKeywords => m_material.shaderKeywords;

        public MaterialContext(Material material)
        {
            if (material == null)
                throw new ArgumentNullException(nameof(material));

            m_material = material;
        }

        public void EnableKeyword(string keyword)
        {
            m_material.EnableKeyword(keyword);
        }

        public void DisableKeyword(string keyword)
        {
            m_material.DisableKeyword(keyword);
        }

        public void SetBool(string varName, bool value)
        {
            m_material.SetInt(varName, value ? 1 : 0);
        }

        public void SetColor(string varName, Color color)
        {
            m_material.SetColor(varName, color);
        }

        public void SetFloat(string varName, float value)
        {
            m_material.SetFloat(varName, value);
        }

        public void SetInteger(string varName, int value)
        {
            m_material.SetInteger(varName, value);
        }

        public void SetMatrix(string varName, Matrix4x4 value)
        {
            m_material.SetMatrix(varName, value);
        }

        public void SetTexture(string varName, Texture value)
        {
            m_material.SetTexture(varName, value);
        }

        public void SetVector(string varName, Vector3 value)
        {
            m_material.SetVector(varName, value);
        }

        public static implicit operator MaterialContext(Material material)
        {
            return new MaterialContext(material);
        }
    }
}
