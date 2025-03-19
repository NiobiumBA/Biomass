#ifndef MANDELBROTINCLUDE
#define MANDELBROTINCLUDE

int _FractalIters;
float _FractalPower;
float4 _FractalColor1;
float4 _FractalColor2;
float _ColorDifference;

float FractalSDF(float3 p)
{
	float3 zn = p;
	float dr = 1.5f;
	float r, teta, phi;
	float rad_pow;
	float cos_phi;
	
	for (int i = 0; i < _FractalIters; i++)
	{
		r = length(zn);
		
		if (r >= 2.0f)
			break;
		
		//zn = abs(zn);
		
		// convert to spherical coordinates
		teta = atan2(zn.y, zn.x);
		phi = asin(zn.z / r);
		
		dr = pow(r, _FractalPower - 1.0f) * _FractalPower * dr + 1.0f;
		
		rad_pow = pow(r, _FractalPower);
		teta *= _FractalPower;
		phi *= _FractalPower;

		// convert back to cartesian coordinates
		cos_phi = cos(phi);
		zn = rad_pow * float3(cos(teta) * cos_phi,
								sin(teta) * cos_phi,
								sin(phi));
		zn += p;
	}
	
	//return max(0.5f * log(r) * r / dr, p.z);
	return 0.5f * log(r) * r / dr;
}

#define FACTOR (1 / log(2.0f))
#define LOGLOGMAXRADIUSSQUARED (log(log(4.0f)))
half3 FractalColor(float3 p)
{
	float3 zn = p;
	float r, teta, phi;
	float rad_pow;
	float cos_phi;
	
	int i;
	for (i = 0; i < _FractalIters; i++)
	{
		r = length(zn);
		
		if (r >= 2.0f)
			break;
		
		// convert to spherical coordinates
		teta = atan2(zn.y, zn.x);
		phi = asin(zn.z / r);
		
		rad_pow = pow(r, _FractalPower);
		teta *= _FractalPower;
		phi *= _FractalPower;

		// convert back to cartesian coordinates
		cos_phi = cos(phi);
		zn = rad_pow * float3(cos(teta) * cos_phi,
								sin(teta) * cos_phi,
								sin(phi));
		zn = zn + float3(-1, 0, 0);
	}
	
	float znSquare = max(dot(zn, zn), 1.5f);
	float smooth = i + (LOGLOGMAXRADIUSSQUARED - log(log(znSquare))) * FACTOR;
	//float smooth = i;
	
	return lerp(_FractalColor1.rgb, _FractalColor2.rgb, saturate(smooth * _ColorDifference));
}
#endif
