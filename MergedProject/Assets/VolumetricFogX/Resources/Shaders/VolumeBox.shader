Shader "Custom/VolumeBox"
{
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
		Cull Front
		Pass
		{
			
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma target 3.5
			#include "UnityCG.cginc"

			uniform sampler3D _VolumeTex;
			uniform sampler2D  _FrontfaceBoxDepth;
			uniform sampler2D  _ScenePos;
			uniform sampler2D  _NoiseTex;
			float4x4  _InvView;
			float4x4  _InvProj;
			float4x4  _InvRot;
			float4 _FogColor;
			uniform float3 vScale;
			uniform float3 vPosition;
			uniform float4 vParameters;
			uniform float4 vWindParameters;
			uniform int _NoOfPointLights;
			uniform float4 vLightParams1[6];
			uniform float4 vLightParams2[6];
			uniform int _NoOfSpotLights;
			uniform float4 vLightParams3[6];
			uniform float4 vLightParams4[6];
			uniform float4 vLightParams5[6];
			struct v2f
			{
				float4 pos      : SV_POSITION;
                float4 scrPos   : TEXCOORD0;
                float3 wViewPos : TEXCOORD1;

			};

			v2f Vert(appdata_base v)
			{   
				v2f o;
                o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
                o.scrPos = o.pos;
                o.scrPos.y *= _ProjectionParams.x;
                o.wViewPos = mul (UNITY_MATRIX_MV, v.vertex).xyz;
                return o;
			}
 			float3 ComputePoint (float3 p1, float3 p2, float3 p3)
			{
				float3 d = normalize(p3 - p2);
				float3 v = p1 - p2;
				float t = abs(dot (v, d));
				return (p2 + t * d);
			}


			float4 Frag(v2f i) : COLOR 
			{
				float2 t = i.scrPos.xy/i.scrPos.w;
				float3 screenPos = float3(i.scrPos.xyz/i.scrPos.w);
                screenPos.xy = (screenPos.xy + float2(1.0f,1.0f)) * 0.5f;
                float3 p2 = i.wViewPos;
                //float4 temp = float4(t.x * i.scrPos.w, t.y * i.scrPos.w, 0.0f, 0.0f);
                //p2 = mul (_InvProj, temp).xyz;
                //p2.z = i.wViewPos.z;
               
                float4 sp = tex2D(_FrontfaceBoxDepth , screenPos.xy);
                float4 temp = float4(t.x * sp.y, t.y * sp.y, 0.0f, 0.0f);
                float3 p1 = mul (_InvProj, temp).xyz;
                p1.z = sp.x;
                //return float4(p1, 0.0f);
                ////////////////////////////////
                //float z = tex2D(_ScenePos , screenPos.xy).x;
                //float4 abc = float4(temp, z, 0.0f);
                //p1 = mul (_InvProj, abc).xyz;
                //return float4(p1, 0.0f);
                ////////////////////////////////
                sp = tex2D(_ScenePos , screenPos.xy);
                temp = float4(t.x * sp.y, t.y * sp.y, 0.0f, 0.0f);
                float3 scenePos = mul (_InvProj, temp).xyz;
                scenePos.z = sp.x;
                //return float4(scenePos, 0.0f);

                float depth = length(p2-p1);
                float sceneDepth = length(scenePos);
                float x = length(p2 - p1);
                float y = length(scenePos - p1);
                if(y < x)
                {
                	depth = y;
                }

                float frontFaceDepth = length(p1);
                if(sceneDepth < frontFaceDepth)
                {
                	return float4(0.0f, 0.0f, 0.0f, 0.0f);
                }

                float3 dir = normalize(p2-p1);
                float3 p = p1;
                float4 a1 = float4(0.0f, 0.0f, 0.0f, 0.0f);
                float4 a2 = float4(0.0f, 0.0f, 0.0f, 0.0f);
               
                int noOfSamples = 32;

                for(int k = 0; k < 32; k++)
                {
                	p += dir * depth/noOfSamples;

                	float3 tex = mul(_InvView, float4(p, 1.0f)).xyz;
                	float3 wPos = tex;
                	tex = mul(_InvRot, float4(tex, 1.0f)).xyz;

                	float2 noiseTex = float2((tex.x - vPosition.x + vScale.x/2.0f)/vScale.x, (tex.z - vPosition.z + vScale.z/2.0f)/vScale.z);
                	float h = tex2D (_NoiseTex, noiseTex).a * vScale.y/2.0f;
                	tex.y -= vPosition.y;
                	if (tex.y >= h || tex.y < -h)
                			continue;
                	tex.xz = float2((tex.x + vScale.x/2.0f)/vParameters.x + vParameters.y * vWindParameters.z * vWindParameters.x, (tex.z + vScale.z/2.0f)/vParameters.x + vParameters.y * vWindParameters.z * vWindParameters.y);
                	tex.y = (tex.y + vScale.y/2.0f)/vScale.y;
                	float4 temp = tex3D(_VolumeTex, tex) * vParameters.z;
                	a2.a = temp.a;
                	a1.a = a1.a + a2.a * (1-a1.a);

					if(length(p - p1) > depth)
						break;
					if(sceneDepth < length(p))
						break;
                }

                float3 lightColor = _FogColor;
                for(int k1 = 0; k1 < 6; k1++)
                {
                	if(k1 == _NoOfPointLights)
                		break;
                	float3 x2 = mul(_InvView, float4(p1, 1.0f)).xyz;
	                float3 x3 = mul(_InvView, float4(p2, 1.0f)).xyz;
	                float3 pp = ComputePoint (vLightParams1[k1].xyz, x2, x3);
	                float3 ppView = mul (UNITY_MATRIX_V, float4(pp, 1.0f)).xyz;
	                x = length(ppView - p1);
	                y = length(scenePos - p1);
	                if(x > y)
	                {
	                	pp = mul(_InvView, float4(scenePos, 1.0f)).xyz;
	                }
	                float lowestDist = length (pp - vLightParams1[k1].xyz)*2.0f / vLightParams2[k1].w; 
	                float lightDistFromCam = length (vLightParams1[k1].xyz - _WorldSpaceCameraPos);

	                if (lowestDist < vLightParams1[k1].w)
	                {
	                	float lightFactor = 1 - min (1.0f, lowestDist/ (vLightParams1[k1].w));
	                	float lightDistFactor = 1.0f - saturate(lightDistFromCam / (vLightParams2[k1].w * 20.0f));
	                	lightFactor *= lightDistFactor;
	                	lightFactor = saturate(lightFactor);
	                	lightColor = vLightParams2[k1].xyz * lightFactor + lightColor * (1 - lightFactor);
	                }	
                }

                float4 ret = float4(lightColor, a1.a);
               
                return ret;
			}


			ENDCG
		}
	}
}
