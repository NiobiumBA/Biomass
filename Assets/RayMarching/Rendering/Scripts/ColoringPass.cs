using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RayMarching.Rendering
{
    public class ColoringPass : ScriptableRenderPass
    {
        private readonly ProfilingSampler m_profilingSampler = new("RayMarchingColoring");
        private Material m_material;
        private MaterialContext m_materialContext;
        private RayMarchingShadows m_shadowsFeature;
        private RTHandle m_cameraColorTarget;
        private RTHandle m_cameraDepthTarget;
        private List<string> m_enabledKeywords = new();

        public ColoringPass(Material material, RayMarchingShadows shadowsFeature)
        {
            m_material = material;
            m_shadowsFeature = shadowsFeature;
            renderPassEvent = RenderPassEvent.AfterRenderingSkybox + 1;

            m_materialContext = new MaterialContext(m_material);
        }

        public void SetTextures(RTHandle colorHandle, RTHandle depthHandle)
        {
            m_cameraColorTarget = colorHandle;
            m_cameraDepthTarget = depthHandle;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if (m_cameraColorTarget != null && m_cameraDepthTarget != null)
                ConfigureTarget(m_cameraColorTarget, m_cameraDepthTarget);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (m_material == null)
                return;

            if (m_cameraColorTarget == null)
                return;

            Camera camera = renderingData.cameraData.camera;

            CommandBuffer cmd = CommandBufferPool.Get();

            using (new ProfilingScope(cmd, m_profilingSampler))
            {
                UpdateParams(camera);

                Blitter.BlitCameraTexture(cmd, m_cameraColorTarget, m_cameraColorTarget, m_material, 0);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        private void UpdateParams(Camera camera)
        {
            RayMarchingUtils.DisableKeywords(m_materialContext);

            SDFSceneManager.ActiveScene?.VisualPart.ApplyContext(m_materialContext, ref m_enabledKeywords);

            if (m_shadowsFeature != null && m_shadowsFeature.isActive)
            {
                m_material.EnableKeyword(RayMarchingShadows.ShadowsEnableKeyword);
            }
            else
            {
                m_material.DisableKeyword(RayMarchingShadows.ShadowsEnableKeyword);
            }

            m_material.SetMatrix("_ViewportToWorld", RayMarchingUtils.GetViewportToWorldMatrix(camera));
            m_material.SetFloat("_RMFactorMinDist", RayMarchingUtils.GetMinDistanceFactor(m_cameraColorTarget.referenceSize));
        }
    }
}