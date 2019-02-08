// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/VolumeFog"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_FilterTex ("Fog Filter", 2D) = "red" {}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform sampler2D _FilterTex;
			uniform float _bwBlend;
			float4 _MainTex_TexelSize;
 
			float4 frag(v2f_img i) : COLOR 
			{
				float2 tex = i.uv;
				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
        			tex.y = 1 - tex.y;
				#endif

				float4 filter = tex2D(_FilterTex, tex);
				float alpha = filter.a;
				float4 lightColor = tex2D(_MainTex, i.uv);

				float4 finalColor = lerp(lightColor, filter, alpha);

				return finalColor;
			}
			ENDCG
		}
	}
}
