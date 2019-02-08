// Upgrade NOTE: replaced 'glstate.matrix.modelview[0]' with 'UNITY_MATRIX_MV'

Shader "Custom/HeightShader" {
     Properties {
	 
         _LowTex ("Low Texture", 2D) = "white" {}
         _HighTex ("High Texture", 2D) = "white" {}
		 _LowBumpMap ("Low Normal Map", 2D) = "bump" {}
		 _HighBumpMap ("High Normal Map", 2D) = "bump" {}
		 
		 _Intensity ("Light Intensity", Float) = 0.75
		 _Height ("Height", Float) = 2.0
		 _BlendRange ("Blend Range", Float) = 2.0
     }
     SubShader {
             Tags { "RenderType"="Opaque" }
             Cull Off
             
             CGPROGRAM
             #pragma surface surf Lambert vertex:vert
			 #pragma target 3.0
			 #pragma debug
             #include <UnityCG.cginc>
			 
             sampler2D _LowTex;
             sampler2D _HighTex;
			 sampler2D _LowBumpMap;
			 sampler2D _HighBumpMap;
			 
			 float _Intensity;
			 float _Height;
			 float _BlendRange;
             
             struct Input{
                 float2 uv_LowTex;
                 float2 uv_HighTex;
				 float2 uv_LowBumpMap;
				 float2 uv_HighBumpMap;
				 
				 float3 worldPos;
				 float normal;
             };
             
             void vert(inout appdata_full v, out Input o){
				UNITY_INITIALIZE_OUTPUT(Input,o);
				o.normal = v.normal.y;
             }
             
             void surf(Input IN, inout SurfaceOutput o){
				half4 tex1;
				half4 tex2;
				half4 bump1;
				half4 bump2;
				float L = 1.0;
				
				L = clamp((IN.worldPos.y - _Height)/_BlendRange, 0, 1);
				tex1 = tex2D(_LowTex, IN.uv_LowTex);
				tex2 = tex2D(_HighTex, IN.uv_HighTex);
				bump1 = tex2D(_LowBumpMap, IN.uv_LowBumpMap);
				bump2 = tex2D(_HighBumpMap, IN.uv_HighBumpMap);
				
				o.Albedo = (tex1.rgb*(1-L) + tex2.rgb*L)*_Intensity;
				o.Normal = UnpackNormal(bump1*(1-L) + bump2*L);
             }
             
             ENDCG
     }
     FallBack "Diffuse"
 }