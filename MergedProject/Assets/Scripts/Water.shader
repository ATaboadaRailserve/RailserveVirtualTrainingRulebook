Shader "Custom/Water" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Large Smoothness", Range(0,1)) = 0.5
		_Color ("Large Metallic", Range(0,1)) = 0.0
		
		_LargeNormal ("Large Normal", 2D) = "white" {}
		_LargeNormalAmount ("Normal Amount", float) = 1
		
		_SmallNormal ("Small Normal", 2D) = "white" {}
		_SmallNormalAmount ("Normal Amount", float) = 1
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
		sampler2D _LargeNormal;
		sampler2D _SmallNormal;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		half _LargeNormalAmount;
		half _SmallNormalAmount;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			fixed4 n = (tex2D (_LargeNormal, IN.uv_MainTex)*_LargeNormalAmount + tex2D (_SmallNormal, IN.uv_MainTex)*_SmallNormalAmount)/2;
			o.Normal = n;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
