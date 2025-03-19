#ifndef SHADOWSUTILS_INCLUDE
#define SHADOWSUTILS_INCLUDE

#include "RayMarchingUtils.hlsl"

// https://iquilezles.org/articles/rmshadows/
float RayMarchingSoftShadow(float3 worldPos, float3 lightDir, float minDist, float lightSize)
{
    float result = 1.0f;
    float lastDist = 1e20f;
    float depth = minDist;
	float dist;
	float y;
	float d;
	
	for (int i = 0; i < RM_ITERATIONS; i++)
    {
        dist = SceneSDF(worldPos - lightDir * depth);
		
		if (dist >= RM_MAXDISTANCE)
			return result;
		
		if (dist <= depth * _RMFactorMinDist)
			return 0;
		
        y = dist * dist / (2.0f * lastDist);
        d = sqrt(abs(dist * dist - y * y));
        //d = sqrt(dist * dist - y * y);
		
        result = min(result, d / (lightSize * max(1e-5f, depth - y)));
        lastDist = dist;
        depth += dist;
    }
	
    return result;
}

#endif
