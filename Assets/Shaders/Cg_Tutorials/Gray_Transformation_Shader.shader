Shader "Cg_Tutorial/Gray_Transformation_Shader"
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
				float g = float((i.col.r + i.col.g + i.col.b) / 3);
				return float4(g, g, g, 0.0);
			}
			ENDCG
		}
	}
}