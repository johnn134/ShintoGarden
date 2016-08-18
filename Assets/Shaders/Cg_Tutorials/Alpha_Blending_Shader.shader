Shader "Cg_Tutorial/Alpha_Blending_Shader"
{
	SubShader
	{
		Tags { "Queue"="Transparent" }

		Pass
		{
			ZWrite Off

			Blend SrcAlpha OneMinusSrcAlpha	//Alpha Blending
			//Blend One OneMinusSrcAlpha		//Premultiplied Alpha Blending
			//Blend One One						//Additive Blending

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4 vert(float4 vertexPos : POSITION) : SV_POSITION
			{
				return mul(UNITY_MATRIX_MVP, vertexPos);
			}
			
			float4 frag(void) : COLOR
			{
				return float4(0.0, 1.0, 0.0, 0.3);
			}

			ENDCG
		}
	}
}
