using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;
using System;

namespace RayMarching.Rendering
{
    public class ShadowsPass : ScriptableRenderPass
    {
        private readonly ProfilingSampler m_profilingSampler = new("RayMarchingShadows");
        private readonly RayMarchingShadows m_shadowsFeature;
        private readonly ComputeShaderContext m_computeShaderContext;
        private readonly int m_kernelId;

        private RTHandle m_shadowsTarget;
        private Vector2Int m_threadGroups;

        private List<string> m_enabledKeywords = new();
        private int m_frameModule = 0;

        public ComputeShader Shader => m_shadowsFeature == null ? null : m_shadowsFeature.ComputeShader;

        public RayMarchingFeature RayMarching => m_shadowsFeature == null ? null : m_shadowsFeature.RayMarching;

        public ShadowsPass(RayMarchingShadows shadowsFeature)
        {
            if (shadowsFeature.ComputeShader == null)
                return;

            m_shadowsFeature = shadowsFeature;
            renderPassEvent = RenderPassEvent.BeforeRenderingOpaques + 1;

            try
            {
                m_kernelId = Shader.FindKernel(shadowsFeature.KernelName);
            }
            catch (ArgumentException)
            {
                m_shadowsFeature = null;
                return;
            }

            if (Shader.IsSupported(m_kernelId) == false)
            {
                m_shadowsFeature = null;
                return;
            }

            m_computeShaderContext = new ComputeShaderContext(Shader, m_kernelId);
        }

        public void SetTextures(RTHandle shadowsTarget)
        {
            if (Shader == null || RayMarching == null)
                return;

            m_shadowsTarget = shadowsTarget;

            Vector2Int threadGroupSize = GetThreadGroupSize();
            Vector2 tile = threadGroupSize * 2;
            m_threadGroups = new Vector2Int(Mathf.CeilToInt(m_shadowsTarget.referenceSize.x / tile.x),
                Mathf.CeilToInt(m_shadowsTarget.referenceSize.y / tile.y));
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (Shader == null || RayMarching == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get();

            using (new ProfilingScope(cmd, m_profilingSampler))
            {
                UpdateParams(ref renderingData);
                cmd.DispatchCompute(Shader, m_kernelId, m_threadGroups.x, m_threadGroups.y, 1);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        private void UpdateParams(ref RenderingData renderingData)
        {
            Camera camera = renderingData.cameraData.camera;

            RayMarchingUtils.DisableKeywords(m_computeShaderContext);

            SDFSceneManager.ActiveScene?.VisualPart.ApplyContext(m_computeShaderContext, ref m_enabledKeywords);

            LightData lightData = renderingData.lightData;
            Light light = lightData.visibleLights[lightData.mainLightIndex].light;
            Vector3 lightDir = light.transform.forward;

            m_shadowsTarget.rt.SetGlobalShaderProperty("_RMShadows");

            Shader.SetTexture(m_kernelId, "_RMShadows", m_shadowsTarget);
            Shader.SetTexture(m_kernelId, "_RMDepth", RayMarching.DepthTexture);
            Shader.SetVector("_RMDepthSize", (Vector2)RayMarching.DepthResolution);
            Shader.SetVector("_RMShadowsSize", (Vector2)m_shadowsTarget.referenceSize);
            Shader.SetVector("_CameraPos", camera.transform.position);
            Shader.SetVector("_LightDir", lightDir);
            Shader.SetFloat("_LightSize", m_shadowsFeature.LightSize);
            Shader.SetInt("_FrameModule", m_frameModule);
            Shader.SetMatrix("_ViewportToWorld", RayMarchingUtils.GetViewportToWorldMatrix(camera));
            Shader.SetFloat("_RMFactorMinDist", RayMarchingUtils.GetMinDistanceFactor(m_shadowsTarget.referenceSize));

            m_frameModule = m_frameModule == 0 ? 1 : 0;
        }

        private Vector2Int GetThreadGroupSize()
        {
            Shader.GetKernelThreadGroupSizes(m_kernelId, out uint x, out uint y, out uint _);
            return new Vector2Int((int)x, (int)y);
        }
    }
}
