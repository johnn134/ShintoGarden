Shader "Cg_Tutorial/HSV_Cylinder_Shader"
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
				float H = 180 + degrees(atan2(vertexPos.z, vertexPos.x));
				float S = 2.0 * sqrt(vertexPos.x * vertexPos.x + vertexPos.z * vertexPos.z);
				float V = (vertexPos.y + 1.0) / 2.0;

				float Hp = H / 60;
				float C = V * S;
				float X = C * (1 - abs(Hp % 2 - 1));
				float m = V - C;

				float3 r;
				if (Hp >= 0 && Hp < 1) {
					r = float3(C, X, 0);
				}
				else if (Hp >= 1 && Hp < 2) {
					r = float3(X, C, 0);
				}
				else if (Hp >= 2 && Hp < 3) {
					r = float3(0, C, X);
				}
				else if (Hp >= 3 && Hp < 4) {
					r = float3(0, X, C);
				}
				else if (Hp >= 4 && Hp < 5) {
					r = float3(X, 0, C);
				}
				else if (Hp >= 5 && Hp < 7.0) {
					r = float3(C, 0, X);
				}
				else {
					r = float3(0, 0, 0);
				}

				o.col = float4(r + m, 1.0);

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
