using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace RayMarching.Rendering
{
    public class RayMarchingShadows : ScriptableRendererFeature
    {
        public const string ShadowsEnableKeyword = "RMSHADOWS_ENABLE";

        [SerializeField] private RayMarchingFeature m_rayMarching;
        [SerializeField] private ComputeShader m_shadowsComputeShader;
        [SerializeField] private string m_kernelName = "ShadowsCompute";
        [SerializeField] private int m_shadowsWidth = 256;
        [SerializeField] private int m_shadowsHeight = 128;
        [SerializeField, Min(0)] private float m_lightSize = 0.2f;

        private ShadowsPass m_shadowsPass;

        private RTHandle m_shadowsTexture;

        public RTHandle ShadowsTexture => m_shadowsTexture;

        public RayMarchingFeature RayMarching => m_rayMarching;

        public ComputeShader ComputeShader => m_shadowsComputeShader;

        public string KernelName => m_kernelName;

        public ComputeShader ShadowsComputeMaterial { get => m_shadowsComputeShader; set => m_shadowsComputeShader = value; }

        public Vector2Int ShadowsResolution
        {
            get => new(m_shadowsWidth, m_shadowsHeight);
            set
            {
                if (value.x <= 0 || value.y <= 0)
                    throw new ArgumentOutOfRangeException();

                m_shadowsWidth = value.x;
                m_shadowsHeight = value.y;

                if (isActive)
                {
                    SetActive(false);
                    SetActive(true);
                }
            }
        }

        public float LightSize
        {
            get => m_lightSize;
            set
            {
                m_lightSize = Mathf.Max(value, 0);
            }
        }

        public RenderTextureDescriptor ShadowsTextureDescriptor => new()
        {
            width = m_shadowsWidth,
            height = m_shadowsHeight,
            dimension = TextureDimension.Tex2D,
            colorFormat = RenderTextureFormat.RFloat,
            msaaSamples = 1,
            depthBufferBits = 0,
            volumeDepth = 1,
            enableRandomWrite = true
        };

        public override void Create()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                RayMarchingUtils.DisableKeywords(new ComputeShaderContext(m_shadowsComputeShader, m_kernelName));
            }
#endif

            m_shadowsPass = new ShadowsPass(this);
        }

        protected override void Dispose(bool disposing)
        {
            m_shadowsTexture?.Release();
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                SDFSceneManager.ActiveScene = SDFSceneManager.CreateFromUnityScene(SceneManager.GetActiveScene());
            }
#endif

            RenderingUtils.ReAllocateIfNeeded(ref m_shadowsTexture, ShadowsTextureDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp);

            m_shadowsPass.SetTextures(m_shadowsTexture);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            CameraType cameraType = renderingData.cameraData.cameraType;

            if (cameraType == CameraType.Preview)
                return;

#if UNITY_EDITOR
            bool isOpenPrefab = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null;

            if (isOpenPrefab && cameraType != CameraType.Game)
                return;
#endif
            
            renderer.EnqueuePass(m_shadowsPass);
        }
    }
}