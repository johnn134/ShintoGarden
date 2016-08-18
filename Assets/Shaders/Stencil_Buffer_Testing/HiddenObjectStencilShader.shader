Shader "StencilTests/HiddenObjectStencilShader"
{
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Geometry+2"}
		LOD 100

		Stencil {
			Ref 1
			Comp Equal
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct vertexInput
			{
				float4 vertex : POSITION;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
			};

			vertexOutput vert (vertexInput input)
			{
				vertexOutput o;

				o.pos = mul(UNITY_MATRIX_MVP, input.vertex);
					
				return o;
			}

			fixed4 frag (vertexOutput input) : COLOR
			{
				return half4(0.5, 0.5, 0.5, 1);
			}
			ENDCG
		}
	}
}
