using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace RayMarching.Rendering
{
    public class RayMarchingFeature : ScriptableRendererFeature
    {
        [SerializeField] private Material m_depthComputeMaterial;
        [SerializeField] private Material m_coloringMaterial;
        [SerializeField] private RayMarchingShadows m_shadowsFeature;
        [SerializeField] private int m_depthWidth = 512;
        [SerializeField] private int m_depthHeight = 256;

        private DepthPass m_depthPass;
        private ColoringPass m_coloringPass;

        private RTHandle m_depthTexture;

        public Material DepthComputeMaterial { get => m_depthComputeMaterial; set => m_depthComputeMaterial = value; }

        public Material ColoringMaterial { get => m_coloringMaterial; set => m_coloringMaterial = value; }

        public RTHandle DepthTexture => m_depthTexture;

        public Vector2Int DepthResolution
        {
            get => new(m_depthWidth, m_depthHeight);
            set
            {
                if (value.x <= 0 || value.y <= 0)
                    throw new ArgumentOutOfRangeException();

                m_depthWidth = value.x;
                m_depthHeight = value.y;

                if (isActive)
                {
                    SetActive(false);
                    SetActive(true);
                }
            }
        }

        public RenderTextureDescriptor DepthTextureDescriptor => new()
        {
            width = m_depthWidth,
            height = m_depthHeight,
            dimension = TextureDimension.Tex2D,
            colorFormat = RenderTextureFormat.RFloat,
            msaaSamples = 1,
            depthBufferBits = 0,
            volumeDepth = 1
        };

        public override void Create()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                RayMarchingUtils.DisableKeywords((MaterialContext)m_coloringMaterial);
                RayMarchingUtils.DisableKeywords((MaterialContext)m_depthComputeMaterial);
            }
#endif

            m_depthPass = new DepthPass(m_depthComputeMaterial);
            m_coloringPass = new ColoringPass(m_coloringMaterial, m_shadowsFeature);
        }

        protected override void Dispose(bool disposing)
        {
            m_depthTexture?.Release();
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                SDFSceneManager.ActiveScene = SDFSceneManager.CreateFromUnityScene(SceneManager.GetActiveScene());
            }
#endif

            RenderingUtils.ReAllocateIfNeeded(ref m_depthTexture, DepthTextureDescriptor, FilterMode.Point, TextureWrapMode.Clamp);

            m_depthPass.SetTextures(m_depthTexture);

            // Calling ConfigureInput with the ScriptableRenderPassInput.Color argument
            // ensures that the opaque texture is available to the Render Pass.
            m_coloringPass.ConfigureInput(ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Depth);
            m_coloringPass.SetTextures(renderer.cameraColorTargetHandle, renderer.cameraDepthTargetHandle);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
#if UNITY_EDITOR
            CameraType cameraType = renderingData.cameraData.cameraType;

            bool isOpenPrefab = PrefabStageUtility.GetCurrentPrefabStage() != null;

            if (isOpenPrefab && cameraType != CameraType.Game)
                return;
#endif

            renderer.EnqueuePass(m_depthPass);
            renderer.EnqueuePass(m_coloringPass);
        }
    }
}