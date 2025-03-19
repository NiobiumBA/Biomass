#ifndef MENGERSPHERE_INCLUDE
#define MENGERSPHERE_INCLUDE

int _FractalIters;

float _SmoothRadius;
float _ScaleFactor;
float _MaxSize;
float _ModOffsetPos;
float4x4 _IterationTransform;

float4 _FractalColor1;
float4 _FractalColor2;
float _ColorDifference;

inline float SphereSDF(float3 pos, float radius)
{
	return length(pos) - radius;
}

inline float CylinderSDF(float2 p, float size)
{
  return length(p) - size;
}

float CylinderCrossSDF(float3 p, float size)
{
  float dxy = CylinderSDF(p.xy, size);
  float dyz = CylinderSDF(p.yz, size);
  float dzx = CylinderSDF(p.zx, size);
  return min(dxy, min(dyz, dzx));
}

float3 mod(float3 x, float3 y)
{
    return x - y * floor(x / y);
}

float3 Repeat(float3 p, float3 size)
{
    float3 halfsize = size * 0.5;
    float3 c = floor((p + halfsize) / size);
    p = fmod(p + halfsize, size) - halfsize;
    p = fmod(p - halfsize, size) + halfsize;
    return p;
}

float FractalSDF(float3 p)
{
	float resultDist = SphereSDF(p, _MaxSize - _SmoothRadius) - _SmoothRadius;
	float currentScale = 1.0;
	
	for (int i = 0; i < _FractalIters; i++)
	{
		p = mul(_IterationTransform, float4(p, 1)).xyz;
		
		float size = _MaxSize * _ModOffsetPos / currentScale;
		
		p = Repeat(p, size);
	  
		currentScale *= _ScaleFactor;
		float3 r = p * currentScale; 
		float currentDist = (CylinderCrossSDF(r, _MaxSize - _SmoothRadius / currentScale) - _SmoothRadius) / currentScale;
		
		resultDist = max(resultDist, -currentDist);
	}
	
	return resultDist;
}

half3 FractalColor(float3 p)
{
	float resultDist = SphereSDF(p, _MaxSize - _SmoothRadius) - _SmoothRadius;
	float currentScale = 1.0;
	
	float3 orbit = 0;
	
	for (int i = 0; i < _FractalIters; i++)
	{
		p = mul(_IterationTransform, float4(p, 1)).xyz;
		
		float size = _MaxSize * _ModOffsetPos * 2 / currentScale;
		
		p = Repeat(p, size);
	  
		currentScale *= _ScaleFactor;
		float3 r = p * currentScale; 
		float currentDist = (CylinderCrossSDF(r, _MaxSize - _SmoothRadius / currentScale) - _SmoothRadius) / currentScale;
		
		orbit = max(orbit, p);
	}
	
	float interpolator = length(orbit);
	
	return lerp(_FractalColor1, _FractalColor2, saturate(interpolator * _ColorDifference)).rgb;
}
#endif
