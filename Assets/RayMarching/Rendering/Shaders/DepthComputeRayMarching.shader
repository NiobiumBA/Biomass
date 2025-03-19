Shader "RayMarching/DepthCompute"
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
			//#pragma multi_compile QUALITY_LOW QUALITY_MEDIUM QUALITY_HIGH QUALITY_ULTRA
			#pragma multi_compile _ MANDELBROT JULIASET MENGERSPHERE PSEUDOKLEINIAN
			#pragma multi_compile _ SCALEFRACTAL
			
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "RayMarchingUtils.hlsl"

            #pragma vertex vert
            #pragma fragment frag
			
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
				float3 dir = normalize(input.viewDir);
				
				float dist;
				bool hit = RayMarch(_WorldSpaceCameraPos, dir, dist);
				
				return hit ? dist : RM_DEPTHNOTHIT;
            }
            ENDHLSL
        }
    }
}