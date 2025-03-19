using System;
using System.Collections.Generic;
using UnityEngine;

namespace RayMarching.Rendering
{
    public class ComputeShaderContext : ISDFRendererContext
    {
        private readonly ComputeShader m_shader;

        public int KernelIndex { get; }

        public ComputeShader Shader => m_shader;

        public IReadOnlyCollection<string> EnabledKeywords => m_shader.shaderKeywords;

        public ComputeShaderContext(ComputeShader shader, int kernelIndex)
        {
            if (shader == null)
                throw new ArgumentNullException(nameof(shader));

            m_shader = shader;
            KernelIndex = kernelIndex;
        }

        public ComputeShaderContext(ComputeShader shader, string kernelName)
        {
            if (shader == null)
                throw new ArgumentNullException(nameof(shader));

            m_shader = shader;
            KernelIndex = shader.FindKernel(kernelName);
        }

        public void EnableKeyword(string keyword)
        {
            m_shader.EnableKeyword(keyword);
        }

        public void DisableKeyword(string keyword)
        {
            m_shader.DisableKeyword(keyword);
        }

        public void SetBool(string varName, bool value)
        {
            m_shader.SetBool(varName, value);
        }

        public void SetColor(string varName, Color color)
        {
            m_shader.SetVector(varName, color);
        }

        public void SetFloat(string varName, float value)
        {
            m_shader.SetFloat(varName, value);
        }

        public void SetInteger(string varName, int value)
        {
            m_shader.SetInt(varName, value);
        }

        public void SetMatrix(string varName, Matrix4x4 value)
        {
            m_shader.SetMatrix(varName, value);
        }

        public void SetTexture(string varName, Texture value)
        {
            m_shader.SetTexture(KernelIndex, varName, value);
        }

        public void SetVector(string varName, Vector3 value)
        {
            m_shader.SetVector(varName, value);
        }
    }
}
