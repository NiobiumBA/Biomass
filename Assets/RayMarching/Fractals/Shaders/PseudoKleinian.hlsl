#ifndef PSEUDOKLEINIAN_INCLUDE
#define PSEUDOKLEINIAN_INCLUDE

int _FractalIters;

float4 CSize;
float Size;
float DEoffset;
float4 C;

float4 _FractalColor1;
float4 _FractalColor2;
float _ColorDifference;

float mod(float x, float y)
{
	return x - y * floor(x / y);
}

float IntersectSDF(float sdf1, float sdf2)
{
	return max(sdf1, sdf2);
}

float BoxSDF(float3 p, float3 b)
{
	float3 q = abs(p) - b;
	return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

float SphereSDF(float3 p, float radius)
{
  return length(p) - radius;
}

// https://github.com/Syntopia/Fragmentarium/blob/master/Fragmentarium-Source/Examples/Knighty%20Collection/PseudoKleinian.frag
float PseudoKleinianSDF(float3 p)
{
	float minD = RM_MAXDISTANCE;
    float3 minXYZ = minD;
	
	p.yz = p.zy;

    float3 or = p;
    float3 ap = p + 1.0;
    //float3 CSize = float3(1.0, 1, 1.3);
    //vec3 CSize = vec3(4.4,2.0,0.5);
    //vec3 CSize = vec3(2.0,1.0,0.3);
    //float Size = 1.75;
    //float DEoffset = 0.0;
    float DEfactor = 2;
    //float3 C = float3(-0.62, -0.015, -0.025);
    //C.xyz = float3(-0.62, -0.015, -0.025);
    //vec3 C = vec3(-0.8,0.1,0.2);
    //vec3 C = vec3(-0.04,0.14,-0.5);
    float4 orbitTrap = minD;
    
    for(int i = 0; i < _FractalIters; i++)
	{
		if (all(ap == p))
			break;
        
		ap = p;
		p = 2.0 * clamp(p, -CSize.xyz, CSize.xyz) - p;
		
		float r2 = dot(p, p);
		orbitTrap = min(orbitTrap, abs(float4(p, r2)));
		float k = max(Size / r2, 1.0);
		p *= k;
		DEfactor *= k;
		
		p += C.xyz;
        orbitTrap = min(orbitTrap, abs(float4(p, dot(p, p))));
        minD = min(minD, length(p - or));
        minXYZ = min(minXYZ, abs(p - or));
	}
    
    //float dist = abs(0.5 * abs(p.z + 0.1) / DEfactor - DEoffset);
    //float dist = abs(0.5 * abs(p.z + 0.1) / DEfactor - 0.001);
    float dist = 0.5f * abs(p.z) / DEfactor;
	return dist;
}

float FractalSDF(float3 p)
{
	//p.x = mod(p.x, 13.5);
	//p.z = mod(p.z, 4.0);

	return max(PseudoKleinianSDF(p), SphereSDF(p, 1));
}

half3 FractalColor(float3 p)
{
	float minD = RM_MAXDISTANCE;
    float3 minXYZ = minD;
	
	p.yz = p.zy;

    float3 or = p;
    float3 ap = p + 1.0;
    float DEfactor = 2;
    float4 orbitTrap = minD;
    
    for(int i = 0; i < _FractalIters; i++)
	{
		if (all(ap == p))
			break;
        
		ap = p;
		p = 2.0 * clamp(p, -CSize.xyz, CSize.xyz) - p;
		
		float r2 = dot(p, p);
		orbitTrap = min(orbitTrap, abs(float4(p, r2)));
		float k = max(Size / r2, 1.0);
		p *= k;
		DEfactor *= k;
		
		p += C.xyz;
        orbitTrap = min(orbitTrap, abs(float4(p, dot(p, p))));
        minD = min(minD, length(p - or));
        minXYZ = min(minXYZ, abs(p - or));
	}
	
	float interpolator = length(minXYZ);
	
	return lerp(_FractalColor1, _FractalColor2, saturate(interpolator * _ColorDifference)).rgb;
}
#endif
