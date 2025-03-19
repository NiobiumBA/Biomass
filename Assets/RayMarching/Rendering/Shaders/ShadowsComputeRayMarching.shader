Shader "RayMarching/ShadowsCompute"
{
	SubShader
    {
        Tags
		{
			"RenderType" = "Opaque"
			"RenderPipeline" = "UniversalPipeline"
		}

        LOD 100
        ZWrite Off
		Cull Off
		
        Pass
        {
			HLSLPROGRAM
			#pragma multi_compile QUALITY_LOW QUALITY_MEDIUM QUALITY_HIGH QUALITY_ULTRA
			#pragma multi_compile _ MANDELBROT JULIASET
			#pragma multi_compile _ SCALEFRACTAL
			
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "RayMarchingUtils.hlsl"
			#include "ColoringUtils.hlsl"

            #pragma vertex vert
            #pragma fragment frag
			
			TEXTURE2D_X(_RMDepth);
            SAMPLER(sampler_RMDepth);
			
			float4 _RMDepth_TexelSize;
			float4 _BlitScaleBias;
			
			struct Attributes
			{
			#if SHADER_API_GLES
				float4 positionOS : POSITION;
				float2 uv : TEXCOORD0;	
			#else
				uint vertexID : SV_VertexID;
			#endif
			};
			
			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float2 texcoord   : TEXCOORD0;
				float3 viewDir : TEXCOORD1;
			};
			
			void InitializeBlitVaryings(Attributes input, inout Varyings values)
			{
			#if SHADER_API_GLES
				float4 pos = input.positionOS;
				float2 uv  = input.uv;
			#else
				float4 pos = GetFullScreenTriangleVertexPosition(input.vertexID);
				float2 uv  = GetFullScreenTriangleTexCoord(input.vertexID);
			#endif

				values.positionCS = pos;
				values.texcoord = uv * _BlitScaleBias.xy + _BlitScaleBias.zw;
				values.viewDir = 0;
			}
			
			Varyings vert(Attributes input)
			{
				Varyings output;
				
				InitializeBlitVaryings(input, output);
				
				float2 viewport = 2 * output.texcoord - 1;
				
				output.viewDir = GetViewDir(viewport, _WorldSpaceCameraPos);
				
				return output;
			}
			
            float frag(Varyings input) : SV_Target
            {
				float3 viewDir = normalize(input.viewDir);
				
				float unityDepth = GetUnityDepth(input.texcoord, input.viewDir);
				float rayDepth = SAMPLE_TEXTURE2D_X(_RMDepth, sampler_RMDepth, input.texcoord.xy).r;
				
				bool doShade = DepthIsHit(unityDepth) || DepthIsHit(rayDepth);
				
				float depth;
				
				if (DepthIsHit(rayDepth))
				{
					if (DepthIsHit(unityDepth))
						depth = min(rayDepth, unityDepth);
					else
						depth = rayDepth;
				}
				else
				{
					depth = unityDepth;
				}
				
				float3 worldPos = _WorldSpaceCameraPos + viewDir * depth;
				
				float3 lightDir = _MainLightPosition.xyz;
				
				float dist;
				bool hit = doShade ? RayMarch(worldPos, lightDir, dist) : 0;
				float shadow = hit ? 0 : 1;
				
				return doShade ? shadow : 0.0f;
            }
            ENDHLSL
        }
    }
}