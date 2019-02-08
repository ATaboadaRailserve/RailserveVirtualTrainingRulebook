Shader "Custom/Outline_Only"
{
	Properties
	{
		_OutlineColor("Outline color", Color) = (0,0,0,1)
		_OutlineWidth("Outline width", Range(0.0,0.5)) = 0.03
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	
	static const fixed4 UP = fixed4(0,1,0,1);
	static const fixed4 DOWN = fixed4(0,-1,0,1);
	static const fixed4 LEFT = fixed4(-1,0,0,1);
	static const fixed4 RIGHT = fixed4(1,0,0,1);
	static const fixed4 FORWARD = fixed4(0,0,1,1);
	static const fixed4 BACKWARD = fixed4(0,0,-1,1);
	
	struct appdata
	{
		float4 vertex : POSITION;
	};

	struct v2f
	{
		float4 pos : POSITION;
	};

	float _OutlineWidth;
	float4 _OutlineColor;

	v2f vertUp(appdata v)
	{
		v.vertex.y += _OutlineWidth;
		
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		return o;
	}
	
	v2f vertDown(appdata v)
	{
		v.vertex.y -= _OutlineWidth;

		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		return o;
	}
	
	v2f vertLeft(appdata v)
	{
		v.vertex.x -= _OutlineWidth;

		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		return o;
	}
	
	v2f vertRight(appdata v)
	{
		v.vertex.x += _OutlineWidth;

		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		return o;
	}
	
	v2f vertForward(appdata v)
	{
		v.vertex.z += _OutlineWidth;

		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		return o;
	}
	
	v2f vertBackward(appdata v)
	{
		v.vertex.z -= _OutlineWidth;

		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		return o;
	}

	ENDCG

	SubShader
	{
		Tags{ "Queue" = "Transparent" }
		
		Pass //Render the Outline
		{
			ZWrite off
			Cull Front

			CGPROGRAM
			#pragma vertex vertUp
			#pragma fragment frag

			half4 frag(v2f i) : COLOR
			{
				return _OutlineColor;
			}
			ENDCG
		}
		
		Pass
		{
			ZWrite off
			Cull Front

			CGPROGRAM
			#pragma vertex vertDown
			#pragma fragment frag

			half4 frag(v2f i) : COLOR
			{
				return _OutlineColor;
			}
			ENDCG
		}
		
		Pass
		{
			ZWrite off
			Cull Front

			CGPROGRAM
			#pragma vertex vertLeft
			#pragma fragment frag

			half4 frag(v2f i) : COLOR
			{
				return _OutlineColor;
			}
			ENDCG
		}
		
		Pass
		{
			ZWrite off
			Cull Front

			CGPROGRAM
			#pragma vertex vertRight
			#pragma fragment frag

			half4 frag(v2f i) : COLOR
			{
				return _OutlineColor;
			}
			ENDCG
		}
		
		Pass
		{
			ZWrite off
			Cull Front

			CGPROGRAM
			#pragma vertex vertForward
			#pragma fragment frag

			half4 frag(v2f i) : COLOR
			{
				return _OutlineColor;
			}
			ENDCG
		}
		
		Pass
		{
			ZWrite off
			Cull Front

			CGPROGRAM
			#pragma vertex vertBackward
			#pragma fragment frag

			half4 frag(v2f i) : COLOR
			{
				return _OutlineColor;
			}
			ENDCG
		}
	}
}
