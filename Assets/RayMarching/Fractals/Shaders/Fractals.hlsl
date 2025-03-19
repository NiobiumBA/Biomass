#ifndef FRACTALSINCLUDE
#define FRACTALSINCLUDE

#if defined(MANDELBROT)
	#include "MandelbrotFractal.hlsl"
#elif defined(JULIASET)
	#include "JuliaSet.hlsl"
#elif defined(MENGERSPHERE)
	#include "MengerSphere.hlsl"
#elif defined(PSEUDOKLEINIAN)
	#include "PseudoKleinian.hlsl"
#else
	inline float FractalSDF(float3 pos)
	{
		return RM_MAXDISTANCE;
	}
	
	inline half3 FractalColor(float3 pos)
	{
		return 0;
	}
#endif

#ifdef SCALEFRACTAL
	float _FractalScale;
	float _FractalInvScale;
#endif

inline float ScaledFractalSDF(float3 pos)
{
#ifdef SCALEFRACTAL
	return FractalSDF(pos * _FractalInvScale) * _FractalScale;
#else
	return FractalSDF(pos);
#endif
}

inline half3 ScaledFractalColor(float3 pos)
{
#ifdef SCALEFRACTAL
	return FractalColor(pos * _FractalInvScale);
#else
	return FractalColor(pos);
#endif
}
#endif
