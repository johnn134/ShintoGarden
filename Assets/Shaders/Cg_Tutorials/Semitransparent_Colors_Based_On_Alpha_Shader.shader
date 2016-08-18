Shader "Cg_Tutorial/Semitransparent_Colors_Based_On_Alpha_Shader"
{
	Properties
	{
		_MainTex ("RGBA Texture Image", 2D) = "white" {}
	}
	SubShader
	{
		Tags {"Queue" = "Transparent"}

		Pass
		{
			Cull Front
			ZWrite Off

			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			uniform sampler2D _MainTex;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
			};
			
			vertexOutput vert (vertexInput input)
			{
				vertexOutput o;

				o.tex = input.texcoord;
				o.pos = mul(UNITY_MATRIX_MVP, input.vertex);

				return o;
			}
			
			float4 frag (vertexOutput input) : COLOR
			{
				float4 color = tex2D(_MainTex, input.tex.xy);
				if(color.a > 0.5)
				{
					color = float4(0.0, 0.0, 0.2, 1.0);
				}
				else
				{
					color = float4(0.0, 0.0, 1.0, 0.3);
				}
				return color;
			}
			ENDCG
		}

		Pass
		{
			Cull Back
			ZWrite Off

			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			uniform sampler2D _MainTex;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
			};
			
			vertexOutput vert (vertexInput input)
			{
				vertexOutput o;

				o.tex = input.texcoord;
				o.pos = mul(UNITY_MATRIX_MVP, input.vertex);

				return o;
			}
			
			float4 frag (vertexOutput input) : COLOR
			{
				float4 color = tex2D(_MainTex, input.tex.xy);
				if(color.a > 0.5)
				{
					//color = float4(0.5 * color.r, 2.0 * color.g, 0.5 * color.b, 1.0);
					color = float4(0.5 * color.r, 1.0 - 0.5 * (1.0 - color.g), 0.5 * color.b, 1.0);
				}
				else
				{
					color = float4(0.0, 0.0, 1.0, 0.3);
				}
				return color;
			}
			ENDCG
		}
	}
	Fallback "Unlit/Transparent"
}
