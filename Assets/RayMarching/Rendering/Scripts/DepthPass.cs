using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RayMarching.Rendering
{
    public class DepthPass : ScriptableRenderPass
    {
        private readonly ProfilingSampler m_profilingSampler = new("RayMarchingDepth");
        private readonly Material m_material;
        private readonly MaterialContext m_materialContext;

        private RTHandle m_depthTarget;
        private List<string> m_enabledKeywords = new();

        public DepthPass(Material material)
        {
            m_material = material;
            renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
            
            m_materialContext = new MaterialContext(m_material);
        }

        public void SetTextures(RTHandle depthHandle)
        {
            m_depthTarget = depthHandle;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (m_material == null)
                return;

            if (m_depthTarget == null)
                return;

            Camera camera = renderingData.cameraData.camera;

            CommandBuffer cmd = CommandBufferPool.Get();

            using (new ProfilingScope(cmd, m_profilingSampler))
            {
                UpdateParams(camera);

                Blitter.BlitCameraTexture(cmd, m_depthTarget, m_depthTarget, m_material, 0);

                m_depthTarget.rt.SetGlobalShaderProperty("_RMDepth");
            }
            context.ExecuteCommandBuffer(cmd);
            
            CommandBufferPool.Release(cmd);
        }

        private void UpdateParams(Camera camera)
        {
            RayMarchingUtils.DisableKeywords(m_materialContext);

            SDFSceneManager.ActiveScene?.VisualPart.ApplyContext(m_materialContext, ref m_enabledKeywords);

            m_material.SetMatrix("_ViewportToWorld", RayMarchingUtils.GetViewportToWorldMatrix(camera));
            m_material.SetFloat("_RMFactorMinDist", RayMarchingUtils.GetMinDistanceFactor(m_depthTarget.referenceSize));
        }
    }
}