Shader "StencilTests/PortalStencilShader"
{
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Geometry+1"}
		LOD 100

		ZWrite Off
		ColorMask 0

		Pass
		{
			Stencil {
				Ref 1
				Comp always
				Pass replace
			}

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
				return half4(0.9, 0.9, 0.9, 1);
			}
			ENDCG
		}
	}
}
