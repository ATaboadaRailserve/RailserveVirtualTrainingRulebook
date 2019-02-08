Shader "Custom/BlendTextures" {
	Properties {
		_WorldHeightBlend ("World Height Blend", float) = 0.0
		_BlendRange ("Blend Range", float) = 1.0
		_HeightEffect ("Height Factor", float) = 1.0
		_HeightPower ("Height Fall Off", float) = 1.0
		
		_ColorOne ("Top Color", Color) = (1,1,1,1)
		_MainTex ("Top Texture (RGB)", 2D) = "white" {}
		_GlossinessOne ("Top Smoothness", 2D) = "white" {}
		_NormalOne ("Top Normal", 2D) = "white" {}
		_NormalOneAmount ("Top Normal Amount", Range(0,1)) = 1
		_HeightOne ("Top Height", 2D) = "white" {}
		_HeightOneAmount ("Top Height Amount", Range(0,1)) = 1
		_OcclusionOne ("Top Occlusion", 2D) = "white" {}
		_OcclusionOneAmount ("Top Occlusion Amount", Range(0,100)) = 1
		
		_ColorTwo ("Bottom Color", Color) = (1,1,1,1)
		_MainTexTwo ("Bottom Texture (RGB)", 2D) = "white" {}
		_GlossinessTwo ("Bottom Smoothness", 2D) = "white" {}
		_NormalTwo ("Bottom Normal", 2D) = "white" {}
		_NormalTwoAmount ("Bottom Normal Amount", Range(0,1)) = 1
		_HeightTwo ("Bottom Height", 2D) = "white" {}
		_HeightTwoAmount ("Bottom Height Amount", Range(0,1)) = 1
		_OcclusionTwo ("Bottom Occlusion", 2D) = "white" {}
		_OcclusionTwoAmount ("Bottom Occlusion Amount", Range(0,100)) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _GlossinessOne;
		sampler2D _NormalOne;
		float _NormalOneAmount;
		sampler2D _HeightOne;
		float _HeightOneAmount;
		sampler2D _OcclusionOne;
		float _OcclusionOneAmount;
		fixed4 _ColorOne;
		
		sampler2D _MainTexTwo;
		sampler2D _GlossinessTwo;
		sampler2D _NormalTwo;
		float _NormalTwoAmount;
		sampler2D _HeightTwo;
		float _HeightTwoAmount;
		sampler2D _OcclusionTwo;
		float _OcclusionTwoAmount;
		fixed4 _ColorTwo;
		
		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
			float3 viewDir;
		};
		
		float _WorldHeightBlend;
		float _BlendRange;
		float _HeightEffect;
		float _HeightPower;
		
		void surf (Input IN, inout SurfaceOutputStandard o) {
			float blend = clamp((IN.worldPos.y + pow(tex2D(_HeightOne, IN.uv_MainTex).r * _HeightEffect, _HeightPower) - _WorldHeightBlend + _BlendRange/2.0)/_BlendRange, 0, 1);
			
			half h = (tex2D (_HeightOne, IN.uv_MainTex).w * blend) + (tex2D (_HeightTwo, IN.uv_MainTex).w * (1.0 - blend));
			float2 offset = ParallaxOffset (h, (tex2D (_HeightOne, IN.uv_MainTex) * _HeightOneAmount * blend) + (tex2D (_HeightTwo, IN.uv_MainTex) * _HeightTwoAmount * (1.0 - blend)), IN.viewDir);
			IN.uv_MainTex += offset;
			
			fixed4 c = (tex2D (_MainTex, IN.uv_MainTex) * _ColorOne * blend * (tex2D (_OcclusionOne, IN.uv_MainTex) * _OcclusionOneAmount)) + (tex2D (_MainTexTwo, IN.uv_MainTex) * _ColorTwo * (1.0 - blend) * (tex2D (_OcclusionTwo, IN.uv_MainTex) * _OcclusionTwoAmount));
			o.Albedo = c.rgb;
			c = (tex2D (_GlossinessOne, IN.uv_MainTex) * blend) + (tex2D (_GlossinessTwo, IN.uv_MainTex) * (1.0 - blend));
			o.Smoothness = dot(c.rgb, float3(0.3, 0.59, 0.11));
			o.Alpha = c.a;
			o.Normal = (UnpackNormal(tex2D (_NormalOne, IN.uv_MainTex)) * blend * _NormalOneAmount) + (UnpackNormal(tex2D (_NormalTwo, IN.uv_MainTex)) * (1.0 - blend) * _NormalTwoAmount);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}