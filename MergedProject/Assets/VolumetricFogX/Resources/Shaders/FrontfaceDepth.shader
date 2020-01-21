// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/FrontfaceDepth"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag

			#include "UnityCG.cginc"

			float4x4  _InvProj;

			struct v2f
			{
				float4 pos      : SV_POSITION;
                float4 scrPos   : TEXCOORD0;
                float3 wPos   : TEXCOORD1;
			};



			v2f Vert(appdata_base v)
			{   
				v2f o;
                o.pos = UnityObjectToClipPos (v.vertex);
                o.scrPos = o.pos;
                //o.scrPos.y *= _ProjectionParams.x;
                o.wPos = mul (UNITY_MATRIX_MV, v.vertex).xyz;
                //o.wPos = mul (o.scrPos, _InvProj).xyz;
                return o;
			}
 
			half4 Frag(v2f i) : COLOR 
			{
                //float4 depth = float4(i.wPos,0.0f);
                //i.scrPos.y *= _ProjectionParams.x;
                half4 depth = half4(i.wPos.z, i.scrPos.w, 0.0f, 0.0f);
                return depth;

			}
			ENDCG
		}
	}
}
