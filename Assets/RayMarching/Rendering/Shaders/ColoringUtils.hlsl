#ifndef COLORINGUTILS
#define COLORINGUTILS

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "RayMarchingUtils.hlsl"

#define BLUR_SHADOWS_RADIUS 1
#define BLUR_SHADOWS_DIAMETR (BLUR_SHADOWS_RADIUS * 2 + 1)

TEXTURE2D_X(_CameraDepthTexture);
SAMPLER(sampler_CameraDepthTexture);

TEXTURE2D_X(_RMShadows);
SAMPLER(sampler_RMShadows);

float4 _RMShadows_TexelSize;

half3 AlphaBlend(half3 sourceCol, half3 addCol, half alpha)
{
	return half3(sourceCol * (1.0f - alpha) + addCol * alpha);
}

float Bilerp(float2 pos, float val00, float val10, float val01, float val11)
{
	float2x2 mtrx = float2x2(val00, val01,
							   val10, val11);
	return mul(mul(float2(1.0f - pos.x, pos.x), mtrx), float2(1.0f - pos.y, pos.y));
}

float GetUnityDepth(float2 uv, float3 viewDir)
{
	float unityDepth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, uv).r; // UNITY_SAMPLE_DEPTH
	
	if (unityDepth == 0.0f)
		return RM_DEPTHNOTHIT;
	
	unityDepth = LinearEyeDepth(unityDepth, _ZBufferParams);
	// Convert from depth buffer (eye space) to true distance from camera
	// This is done by multiplying the eyespace depth by the length of the "z-normalized"
	// ray (see vert()).  Think of similar triangles: the view-space z-distance between a point
	// and the camera is proportional to the absolute distance.
	return unityDepth * length(viewDir);
}

float GetSoftShadow(float2 uv)
{
	// if (uv.x > 0.5f)
		// return SAMPLE_TEXTURE2D_X(_RMShadows, sampler_RMShadows, uv).r;

	float sum = 0;

	[unroll] for (int i = 0; i < BLUR_SHADOWS_DIAMETR; i++)
	{
		[unroll] for (int j = 0; j < BLUR_SHADOWS_DIAMETR; j++)
		{
			float2 offset = _RMShadows_TexelSize.xy * (float2(i, j) - BLUR_SHADOWS_RADIUS);
			
			sum += SAMPLE_TEXTURE2D_X(_RMShadows, sampler_RMShadows, uv + offset).r;
		}
	}
	return sum / (BLUR_SHADOWS_DIAMETR * BLUR_SHADOWS_DIAMETR);
}

half4 MyUniversalFragmentPBR(InputData inputData, SurfaceData surfaceData)
{
    #if defined(_SPECULARHIGHLIGHTS_OFF)
    bool specularHighlightsOff = true;
    #else
    bool specularHighlightsOff = false;
    #endif
    BRDFData brdfData;

    // NOTE: can modify "surfaceData"...
    InitializeBRDFData(surfaceData, brdfData);

    #if defined(DEBUG_DISPLAY)
    half4 debugColor;

    if (CanDebugOverrideOutputColor(inputData, surfaceData, brdfData, debugColor))
    {
        return debugColor;
    }
    #endif

    // Clear-coat calculation...
    BRDFData brdfDataClearCoat = CreateClearCoatBRDFData(surfaceData, brdfData);
    half4 shadowMask = CalculateShadowMask(inputData);
    AmbientOcclusionFactor aoFactor = CreateAmbientOcclusionFactor(inputData, surfaceData);
    uint meshRenderingLayers = GetMeshRenderingLayer();
    Light mainLight = GetMainLight(inputData, shadowMask, aoFactor);
	mainLight.shadowAttenuation = inputData.shadowMask;

    // NOTE: We don't apply AO to the GI here because it's done in the lighting calculation below...
    MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI);

    LightingData lightingData = CreateLightingData(inputData, surfaceData);

    lightingData.giColor = GlobalIllumination(brdfData, brdfDataClearCoat, surfaceData.clearCoatMask,
                                              inputData.bakedGI, aoFactor.indirectAmbientOcclusion, inputData.positionWS,
                                              inputData.normalWS, inputData.viewDirectionWS, inputData.normalizedScreenSpaceUV);
#ifdef _LIGHT_LAYERS
    if (IsMatchingLightLayer(mainLight.layerMask, meshRenderingLayers))
#endif
    {
        lightingData.mainLightColor = LightingPhysicallyBased(brdfData, brdfDataClearCoat,
                                                              mainLight,
                                                              inputData.normalWS, inputData.viewDirectionWS,
                                                              surfaceData.clearCoatMask, specularHighlightsOff);
    }

    #if defined(_ADDITIONAL_LIGHTS)
    uint pixelLightCount = GetAdditionalLightsCount();

    #if USE_FORWARD_PLUS
    for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++)
    {
        FORWARD_PLUS_SUBTRACTIVE_LIGHT_CHECK

        Light light = GetAdditionalLight(lightIndex, inputData, shadowMask, aoFactor);

#ifdef _LIGHT_LAYERS
        if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
#endif
        {
            lightingData.additionalLightsColor += LightingPhysicallyBased(brdfData, brdfDataClearCoat, light,
                                                                          inputData.normalWS, inputData.viewDirectionWS,
                                                                          surfaceData.clearCoatMask, specularHighlightsOff);
        }
    }
    #endif

    LIGHT_LOOP_BEGIN(pixelLightCount)
        Light light = GetAdditionalLight(lightIndex, inputData, shadowMask, aoFactor);

#ifdef _LIGHT_LAYERS
        if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
#endif
        {
            lightingData.additionalLightsColor += LightingPhysicallyBased(brdfData, brdfDataClearCoat, light,
                                                                          inputData.normalWS, inputData.viewDirectionWS,
                                                                          surfaceData.clearCoatMask, specularHighlightsOff);
        }
    LIGHT_LOOP_END
    #endif

    #if defined(_ADDITIONAL_LIGHTS_VERTEX)
    lightingData.vertexLightingColor += inputData.vertexLighting * brdfData.diffuse;
    #endif

#if REAL_IS_HALF
    // Clamp any half.inf+ to HALF_MAX
    return min(CalculateFinalColor(lightingData, surfaceData.alpha), HALF_MAX);
#else
    return CalculateFinalColor(lightingData, surfaceData.alpha);
#endif
}

#endif
