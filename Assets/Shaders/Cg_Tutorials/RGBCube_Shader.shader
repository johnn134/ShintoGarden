Shader "Cg_Tutorial/RGBCube_Shader"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct v2f {
				float4 pos : SV_POSITION;
				float4 col : TEXCOORD0;
			};

			v2f vert(float4 vertexPos : POSITION)
			{
				v2f o;

				o.pos = mul(UNITY_MATRIX_MVP, vertexPos);
				o.col = vertexPos + float4(0.5, 0.5, 0.5, 0.0);

				return o;
			}

			float4 frag(v2f i) : COLOR
			{
				return i.col;
			}
			ENDCG
		}
	}
}
