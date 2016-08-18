Shader "Cg_Tutorial/Two_Pass_Discard_Shader"
{
	SubShader
	{
		Pass
		{
			Cull Front

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct vinput
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 posInObjectCoords : TEXCOORD0;
			};
			
			v2f vert (vinput i)
			{
				v2f o;

				o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
				o.posInObjectCoords = i.vertex;

				return o;
			}
			
			float4 frag (v2f i) : COLOR
			{
				if(i.posInObjectCoords.y > 0.0) 
				{
					discard;
				}
				return float4(1.0, 0.0, 0.0, 1.0);
			}
			ENDCG
		}

		Pass
		{
			Cull Back

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct vinput
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 posInObjectCoords : TEXCOORD0;
			};
			
			v2f vert (vinput i)
			{
				v2f o;

				o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
				o.posInObjectCoords = i.vertex;

				return o;
			}
			
			float4 frag (v2f i) : COLOR
			{
				if(i.posInObjectCoords.y > 0.0) 
				{
					discard;
				}
				return float4(0.0, 1.0, 0.0, 1.0);
			}
			ENDCG
		}
	}
}
