#ifndef RAYMARCHINGUTILS
#define RAYMARCHINGUTILS

#ifdef QUALITY_LOW
	#define RM_ITERATIONS 256
#elif QUALITY_MEDIUM
	#define RM_ITERATIONS 256
#elif QUALITY_HIGH
	#define RM_ITERATIONS 256
#elif QUALITY_ULTRA
	#define RM_ITERATIONS 512
#else
	#define RM_ITERATIONS 256
#endif

#define RM_MAXDISTANCE 256
#define RM_DEPTHNOTHIT -1.0f

#include "Assets/RayMarching/Fractals/Shaders/Fractals.hlsl"


float4x4 _ViewportToWorld;
float _RMFactorMinDist;


// return value isn't normalized
float3 GetViewDir(float2 viewPort, float3 cameraPos)
{
	float4 viewPort4 = float4(viewPort, -1.0f, 1.0f);
	
	float4 nearClipPlanePos = mul(_ViewportToWorld, viewPort4);
	nearClipPlanePos.xyz *= 1.0f / nearClipPlanePos.w;
	
	return nearClipPlanePos.xyz - cameraPos;
}

// Signed distance function
inline float SceneSDF(float3 pos)
{
	return ScaledFractalSDF(pos);
}

inline half3 SceneColor(float3 pos)
{
	return ScaledFractalColor(pos);
}

inline bool DepthIsHit(float depth)
{
	return depth != RM_DEPTHNOTHIT;
}

// return ray intersects
// else depth is equals RM_DEPTHNOTHIT
bool RayMarch(float3 pos, float3 dir, out float depth)
{
	depth = 0.0f;
	// If we took too many steps and didn't go too far,
	// it means that we were very close to the object and 
	// therefore we return depth with min distance
	float minDist = RM_MAXDISTANCE;
	float depthWithMinDist = 0.0f;
	
	for (int i = 0; i < RM_ITERATIONS; i++)
	{
		float dist = max(SceneSDF(pos), 0.0f);
		
		if (dist <= depth * _RMFactorMinDist)
			return true;
		
		depth += dist;
		
		if (dist >= RM_MAXDISTANCE)
			return false;
		
		if (minDist > dist)
		{
			minDist = dist;
			depthWithMinDist = depth;
		}
		
		pos += dir * dist;
	}
	
	depth = depthWithMinDist;
	return true;
}

float3 SceneNormal(float3 pos, float delta)
{
	float2 deltaVec = float2(delta, 0);
	
	return normalize(
		float3(
			SceneSDF(pos + deltaVec.xyy) - SceneSDF(pos - deltaVec.xyy),
			SceneSDF(pos + deltaVec.yxy) - SceneSDF(pos - deltaVec.yxy),
			SceneSDF(pos + deltaVec.yyx) - SceneSDF(pos - deltaVec.yyx)
		)
	);
}
#endif
