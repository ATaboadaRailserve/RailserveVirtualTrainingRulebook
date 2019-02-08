Shader "Custom/SceneDepth"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag

			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos      : SV_POSITION;
                float4 scrPos   : TEXCOORD0;
                float3 wPos   : TEXCOORD1;
			};

			v2f Vert(appdata_base v)
			{   
				v2f o;
                o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
                o.scrPos = o.pos;
                o.wPos = mul (UNITY_MATRIX_MV, v.vertex).xyz;
                return o;
			}


 
			half4 Frag(v2f i) : COLOR 
			{
                //float4 depth = float4(i.wPos,0.0f);
                //i.scrPos.y *= _ProjectionParams.x;
                //float4 depth = mul(_InvProj, i.scrPos);

                half4 depth = half4(i.wPos.z, i.scrPos.w, 0.0f, 0.0f);
                return depth;
			}
			ENDCG
		}
	}
}
