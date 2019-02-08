Shader "Custom/BlendTextures_Mobile" {
	Properties {
		_WorldHeightBlend ("World Height Blend", float) = 0.0
		_BlendRange ("Blend Range", float) = 1.0
		_HeightEffect ("Height Factor", float) = 1.0
		_HeightPower ("Height Fall Off", float) = 1.0
		
		_ColorOne ("Top Color", Color) = (1,1,1,1)
		_MainTex ("Top Texture (RGB)", 2D) = "white" {}
		
		_ColorTwo ("Bottom Color", Color) = (1,1,1,1)
		_MainTexTwo ("Bottom Texture (RGB)", 2D) = "white" {}
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
		fixed4 _ColorOne;
		
		sampler2D _MainTexTwo;
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
			
			float blend = clamp((IN.worldPos.y  - _WorldHeightBlend + _BlendRange/2.0)/_BlendRange, 0, 1);
			
			fixed4 c = (tex2D (_MainTex, IN.uv_MainTex) * _ColorOne * blend) + (tex2D (_MainTexTwo, IN.uv_MainTex) * _ColorTwo * (1.0 - blend));
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}