//This shader goes on the objects themselves. It just draws the object as white, and has the "Outline" tag.
 
Shader "Custom/DrawSimple"
{
Properties
	{
		_Color("Main Color",Color)= (0.5,0.5,0.5,1)
		_MainTex ("Texture", 2D) = "white" {}
	}
    SubShader 
    {
         ZWrite On
       // ZTest Always
        Lighting Off
        Pass
        {
            CGPROGRAM
            #pragma vertex VShader
            #pragma fragment FShader
 
            struct VertexToFragment
            {
                float4 pos:SV_POSITION;
            };
 
            //just get the position correct
            VertexToFragment VShader(VertexToFragment i)
            {
                VertexToFragment o;
                o.pos=mul(UNITY_MATRIX_MVP,i.pos);
                return o;
            }
 
            //return white
            half4 FShader():COLOR0
            {
                return half4(1,1,1,0);
            }
 
            ENDCG
        }
        Pass //Normal render
		{
			ZWrite On

			Material
			{
				Diffuse[_Color]
				Ambient[_Color]
			}

			Lighting On

			SetTexture[_MainTex]
			{
				ConstantColor[_Color]
			}

			SetTexture[_MainTex]
			{
				Combine previous * primary DOUBLE
			}
		}
    }
}