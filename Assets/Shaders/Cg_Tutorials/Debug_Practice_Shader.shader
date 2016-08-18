Shader "Cg_Tutorial/Debug_Practice_Shader"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				float4 col : TEXCOORD0;
			};

			v2f vert(appdata_full input)
			{
				v2f o;

				o.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				//o.col = float4(input.texcoord.x, 0.0, 0.0, 1.0);
				//o.col = float4(0.0, input.texcoord.y, 0.0, 1.0);
				//o.col = float4((input.normal + float3(1.0, 1.0, 1.0)) / 2.0, 1.0);
				//o.col = input.texcoord - float4(0.5, 0.3, 0.1, 0.0);
				//o.col = input.texcoord.z;
				//o.col = input.texcoord / tan(0.5);
				o.col = float4(cross(input.normal, input.vertex.xyz), 1.0);

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
