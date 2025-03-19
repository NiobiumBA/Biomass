Shader "Custom/ColorSelectorPanel"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Hue ("Hue", Range(0, 1)) = 0
    }
    SubShader
    {
        Cull Back ZWrite On ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
			
			CBUFFER_START(UnityPerMaterial)
				float4 _MainTex_ST;
				float _Hue;
			CBUFFER_END
			
			half3 HSVtoRGB(half3 hsv)
			{
				if (hsv.y == 0.0)
					return hsv.z;

				half f = frac(hsv.x) * 6;
				uint i = f % 6;
				half f1 = f - i;
				half p = hsv.z * (1.0 - hsv.y);
				half q = hsv.z * (1.0 - f1 * hsv.y);
				half t = hsv.z * (1.0 - (1.0 - f1) * hsv.y);
				
				half3 result;
				
				if (i == 0)
					result = half3(hsv.z, t, p);
				else if (i == 1)
					result = half3(q, hsv.z, p);
				else if (i == 2)
					result = half3(p, hsv.z, t);
				else if (i == 3)
					result = half3(p, q, hsv.z);
				else if (i == 4)
					result = half3(t, p, hsv.z);
				else
					result = half3(hsv.z, p, q);
					
				return result;
			}

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
				half3 hsv = half3(_Hue, i.uv);
                half3 col = HSVtoRGB(hsv);
				
                return half4(col, 1);
            }
            ENDHLSL
        }
    }
}
