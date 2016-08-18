Shader "4DShaders/W_Pos_Shader"
{
	Properties
	{
		_WPos ("Position on the 4th dimension", Int) = 0
	}
	SubShader
	{
		Pass
		{
			Tags { "Queue"="Opaque" "LightMode"="ForwardBase" }

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			uniform int _WPos;

			struct vertexInput
			{
				float4 vertex : POSITION;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 col : COLOR;
			};

			vertexOutput vert (vertexInput input)
			{
				vertexOutput o;

				o.pos = mul(UNITY_MATRIX_MVP, input.vertex);

				o.col = float4(clamp(0.0, 1.0, ((_WPos + 3) % 6) - 2), 
							   clamp(0.0, 1.0, ((_WPos + 1) % 6) - 2) + max(0.0, (1 - ((_WPos + 5) % 6))) * 0.5, 
							   clamp(0.0, 1.0, _WPos - 3), 
							   1.0);
					
				return o;
			}

			float4 frag (vertexOutput input) : COLOR
			{
				return input.col;
			}
			ENDCG
		}
	}
}
