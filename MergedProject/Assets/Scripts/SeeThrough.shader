Shader "Custom/SeeThrough" {
	Properties {
		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		
		_OccTex ("Overlay Texture", 2D) = "white" {}
		_OccTexTile ("Overlay Tiling", Float) = 1
		_OccColor ("Overlay Color", Color) = (1,1,1,1)
		_NonOccExponent ("Alpha Exponent", Float) = 1
		[MaterialToggle] _InvertAlpha("Invert Alpha", Float) = 0
		_OccExponent ("Occlusion Alpha Exponent", Float) = 1
		[MaterialToggle] _InvertOccAlpha("Invert Occluded Alpha", Float) = 0
		[MaterialToggle] _ScreenSpace("Screen Space UV", Float) = 1
		
		_DistanceDivisor ("Distance Divisor", Float) = 10
		_DistanceExponent ("Distance Exponent", Float) = 0.5
		_DistanceMin ("Minimum Distance", Range(0,1)) = 0.2
	}
    SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200
		
		// Render the Object like any other
		CGPROGRAM
		#pragma surface surf ToonRamp alpha
		
		sampler2D _Ramp;
		float4 _Color;
		
		// custom lighting function that uses a texture ramp based
		// on angle between light direction and normal
		#pragma lighting ToonRamp exclude_path:prepass
		inline half4 LightingToonRamp (SurfaceOutput s, half3 lightDir, half atten)
		{
			#ifndef USING_DIRECTIONAL_LIGHT
			lightDir = normalize(lightDir);
			#endif
			
			half d = dot (s.Normal, lightDir)*0.5 + 0.5;
			half3 ramp = tex2D (_Ramp, float2(d,d)).rgb;
			
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
			c.a = s.Alpha;
			return c;
		}
		
		sampler2D _MainTex;
		
		struct Input {
			float2 uv_MainTex : TEXCOORD0;
		};
		
		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
		
		
		
		
		// What overlays when the object is Occluded
		Cull Back
		ZTest LEqual
		
		CGPROGRAM
		#pragma surface surf Lambert alpha
		
		sampler2D _OccTex;
		float4 _OccColor;
		float _OccTexTile;
		float _NonOccExponent;
		float _InvertAlpha;
		float _DistanceDivisor;
		float _DistanceExponent;
		float _DistanceMin;
		float _ScreenSpace;
		
		struct Input {
			float2 uv_OccTex : TEXCOORD0;
			float4 screenPos;
			float3 worldNormal;
			float3 viewDir;
		};
		
		void surf (Input IN, inout SurfaceOutput o) {
		
			o.Albedo = (1-_ScreenSpace)*(tex2D (_OccTex, IN.uv_OccTex * _OccTexTile).rgb * _OccColor.rgb) + _ScreenSpace*(_OccColor.rgb);
			o.Alpha = (1-_ScreenSpace)*(tex2D (_OccTex, IN.uv_OccTex * _OccTexTile).a * _OccColor.a) + _ScreenSpace*(_OccColor.a);
			
			float t = pow(clamp(_InvertAlpha-(((_InvertAlpha-0.5)*2)*(dot(IN.viewDir, IN.worldNormal))),0,1),_NonOccExponent);
			o.Alpha *= t/pow(t, _ScreenSpace);
			
			float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
			screenUV *= float2(8*_OccTexTile,6*_OccTexTile);
			
			float3 t2 = tex2D (_OccTex, screenUV).rgb * 2;
			o.Albedo *= t2/pow(t2, 1-_ScreenSpace);
			
			t = tex2D (_OccTex, screenUV).a;
			t *= pow(clamp(_InvertAlpha-(((_InvertAlpha-0.5)*2)*(dot(IN.viewDir, IN.worldNormal))),0,1),_NonOccExponent);
			t *= pow(clamp(IN.screenPos.z/_DistanceDivisor, _DistanceMin, 1),_DistanceExponent);
			o.Alpha *= (_ScreenSpace==1 ? t/pow(t, 1-_ScreenSpace) : 1);
			
			o.Alpha = clamp(o.Alpha, 0, 1);
			
		}
		ENDCG
		
		
		
		
		// What overlays when the object isn't Occluded
		ZTest Greater
		
		CGPROGRAM
		#pragma surface surf Lambert alpha
		
		sampler2D _OccTex;
		float4 _OccColor;
		float _OccTexTile;
		float _OccExponent;
		float _InvertOccAlpha;
		float _DistanceDivisor;
		float _DistanceExponent;
		float _DistanceMin;
		float _ScreenSpace;
		
		struct Input {
			float2 uv_OccTex : TEXCOORD0;
			float4 screenPos;
			float3 worldNormal;
			float3 viewDir;
		};
		
		void surf (Input IN, inout SurfaceOutput o) {
		
			o.Albedo = (1-_ScreenSpace)*(tex2D (_OccTex, IN.uv_OccTex * _OccTexTile).rgb * _OccColor.rgb) + _ScreenSpace*(_OccColor.rgb);
			o.Alpha = (1-_ScreenSpace)*(tex2D (_OccTex, IN.uv_OccTex * _OccTexTile).a * _OccColor.a) + _ScreenSpace*(_OccColor.a);
			
			float t = pow(clamp(_InvertOccAlpha-(((_InvertOccAlpha-0.5)*2)*(dot(IN.viewDir, IN.worldNormal))),0,1),_OccExponent);
			o.Alpha *= t/pow(t, _ScreenSpace);
			
			float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
			screenUV *= float2(8*_OccTexTile,6*_OccTexTile);
			
			float3 t2 = tex2D (_OccTex, screenUV).rgb * 2;
			o.Albedo *= t2/pow(t2, 1-_ScreenSpace);
			
			t = tex2D (_OccTex, screenUV).a;
			t *= pow(clamp(_InvertOccAlpha-(((_InvertOccAlpha-0.5)*2)*(dot(IN.viewDir, IN.worldNormal))),0,1),_OccExponent);
			t *= pow(clamp(IN.screenPos.z/_DistanceDivisor, _DistanceMin, 1),_DistanceExponent);
			o.Alpha *= (_ScreenSpace==1 ? t/pow(t, 1-_ScreenSpace) : 1);
			
			o.Alpha = clamp(o.Alpha, 0, 1);
			
		}
		ENDCG
	}
	Fallback "Diffuse"
}