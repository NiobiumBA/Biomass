Shader "RayMarching/Coloring"
{
	Properties
	{
		_Smoothness("Smoothness", Range(0,1)) = 0
        _Metallic("Metallic", Range(0,1)) = 0
		_Occlision("Occlision", Range(0, 1)) = 1
	}
	SubShader
    {
        Tags
		{
			"RenderType" = "Opaque"
			"RenderPipeline" = "UniversalPipeline"
		}

        LOD 100
        ZWrite On
		Cull Off
		
        Pass
        {
            HLSLPROGRAM
			//#pragma multi_compile QUALITY_LOW QUALITY_MEDIUM QUALITY_HIGH QUALITY_ULTRA
			#pragma multi_compile _ MANDELBROT JULIASET MENGERSPHERE PSEUDOKLEINIAN
			#pragma multi_compile _ SCALEFRACTAL
			#pragma multi_compile _ RMSHADOWS_ENABLE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS
			#pragma multi_compile_lightpass
			
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "RayMarchingUtils.hlsl"
			#include "ColoringUtils.hlsl"

            #pragma vertex vert
            #pragma fragment frag

            TEXTURE2D_X(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);
			
            TEXTURE2D_X(_RMDepth);
            SAMPLER(sampler_RMDepth);
			
			float4 _RMDepth_TexelSize;
			float4 _BlitScaleBias;
			
			CBUFFER_START(UnityPerMaterial)
				float _Smoothness;
				float _Metallic;
				float _Occlision;
			CBUFFER_END
			
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
			
			struct FragmentOut
			{
				half4 color : SV_Target;
				float depth : SV_Depth;
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
			
			inline float GetRayMarchingDepth(int2 texelPos)
			{
				return SAMPLE_TEXTURE2D_X(_RMDepth, sampler_RMDepth, texelPos * _RMDepth_TexelSize.xy).r;
			}
			
			half3 CalculateLighting(float2 uv, half3 albedo, float3 viewDir, float3 positionWS, float minDist)
			{
				float3 normal = SceneNormal(positionWS, minDist);
				
				#ifdef RMSHADOWS_ENABLE
					float shadowAttenuation = GetSoftShadow(uv);
				#else
					float shadowAttenuation = 1;
				#endif
				
				InputData inputData = (InputData)0;
				inputData.positionWS = positionWS;
				inputData.normalWS = normal;
				inputData.shadowMask = shadowAttenuation;
				inputData.viewDirectionWS = -viewDir;
				inputData.bakedGI = 1;//SAMPLE_GI(i.lightmapUV, i.vertexSH, inputdata.normalWS);

				SurfaceData surfaceData = (SurfaceData)0;
				surfaceData.albedo = albedo;
				surfaceData.metallic = _Metallic;
				surfaceData.smoothness = _Smoothness;
				surfaceData.occlusion = _Occlision;
				surfaceData.alpha = 1;
				
				unity_LightData.z = 1;
				unity_LightData.y = _AdditionalLightsCount.x;
				
				return MyUniversalFragmentPBR(inputData, surfaceData).rgb;
				
				//half4 shadowMask = CalculateShadowMask(inputData);
				//AmbientOcclusionFactor aoFactor = CreateAmbientOcclusionFactor(inputData, surfaceData);
				//Light mainLight = GetMainLight(inputData, shadowMask, aoFactor);
				//Light mainLight = GetMainLight();
				//mainLight.distanceAttenuation = 1;
				
				//half3 attenuatedLightColor = mainLight.color * (mainLight.distanceAttenuation * mainLight.shadowAttenuation);
				//half3 lightDiffuseColor = LightingLambert(attenuatedLightColor, mainLight.direction, inputData.normalWS);
				//return GetAdditionalLightsCount();
				//return _AdditionalLightsCount.x;
				//return CalculateBlinnPhong(mainLight, inputData, surfaceData);
			}
			
			// viewDir isn't normalized
			float GetBilerpRayDepth(float2 uv, float3 viewDir, out half boundAA)
			{
				float2 resolution = _RMDepth_TexelSize.zw;
				
				float2 pos = uv * resolution;
				
				float centerDepth = GetRayMarchingDepth(pos);
				if (DepthIsHit(centerDepth) == false)
				{
					boundAA = 0;
					return centerDepth;
				}
				
				int2 buffer2DPos = pos - 0.5f;
				float2 shift = pos - 0.5f - buffer2DPos;
				
				int2 buffer2DPos11 = buffer2DPos + 1;
				int2 buffer2DPos01 = int2(buffer2DPos.x, buffer2DPos11.y);
				int2 buffer2DPos10 = int2(buffer2DPos11.x, buffer2DPos.y);
				
				float depth00 = GetRayMarchingDepth(buffer2DPos);
				float depth01 = GetRayMarchingDepth(buffer2DPos01);
				float depth10 = GetRayMarchingDepth(buffer2DPos10);
				float depth11 = GetRayMarchingDepth(buffer2DPos11);
				
				bool boundAA00 = DepthIsHit(depth00);
				bool boundAA01 = DepthIsHit(depth01);
				bool boundAA10 = DepthIsHit(depth10);
				bool boundAA11 = DepthIsHit(depth11);
				
				if (boundAA00 == false)
					depth00 = centerDepth;
				if (boundAA01 == false)
					depth01 = centerDepth;
				if (boundAA10 == false)
					depth10 = centerDepth;
				if (boundAA11 == false)
					depth11 = centerDepth;
				
				boundAA = Bilerp(shift, boundAA00, boundAA10, boundAA01, boundAA11);
				
				return Bilerp(shift, depth00, depth10, depth01, depth11);
			}
			
			
			
			Varyings vert(Attributes input)
			{
				Varyings output;
				
				InitializeBlitVaryings(input, output);
				
				float2 viewport = 2 * output.texcoord - 1;
				
				output.viewDir = GetViewDir(viewport, _WorldSpaceCameraPos);
				// Dividing by z "normalizes" it in the z axis of camera.
				// Z-axis of a camera is the length of view direction of center
				output.viewDir *= 1.0f / length(GetViewDir(0.0f, _WorldSpaceCameraPos));
				
				return output;
			}
			
            FragmentOut frag(Varyings input)
            {
				FragmentOut output;
			
				float unityDepth = GetUnityDepth(input.texcoord, input.viewDir);
				
				half4 opaqueColor = SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, input.texcoord);
				
				half boundAA;
				float rayDepth = GetBilerpRayDepth(input.texcoord, input.viewDir, boundAA);
				
				float3 viewDir = normalize(input.viewDir);
				float3 rayEnd = _WorldSpaceCameraPos + rayDepth * viewDir;
				float minDist = rayDepth * _RMFactorMinDist;
				
				//half alphaBoundary = isBound ? saturate(1.5f - distOverMinDist) : 1.0;
				half alphaBoundary = boundAA * boundAA * boundAA;
				//half alphaUnityDepth = saturate(abs(rayDepth - unityDepth) - minDist);
				
				half alpha = alphaBoundary; //min(alphaBoundary, alphaUnityDepth);
				
				half3 rayColor = SceneColor(rayEnd);
				
				half3 colorWithLight = CalculateLighting(input.texcoord, rayColor, viewDir, rayEnd, minDist);
				//half3 colorWithLight = SAMPLE_TEXTURE2D_X(_RMShadows, sampler_RMShadows, input.texcoord).r;
				
				half3 resultColor = AlphaBlend(opaqueColor.rgb, colorWithLight, alpha);
				
				bool rayIsHit = DepthIsHit(rayDepth);
				bool doColoring = rayIsHit && (DepthIsHit(unityDepth) == false || rayDepth < unityDepth);
				
				float rayOutputDepth = Linear01Depth(rayDepth, _ZBufferParams);
				float unityOutputDepth = DepthIsHit(unityDepth) ? Linear01Depth(unityDepth, _ZBufferParams) : Linear01Depth(0, _ZBufferParams);
				
				output.color = doColoring ? half4(resultColor, opaqueColor.a) : opaqueColor;
				output.depth = doColoring ? rayOutputDepth : unityOutputDepth;
				
				return output;
            }
            ENDHLSL
        }
    }
}