Shader "Scroll/ScrollVisibleObjectCombinedShader"
{
	Properties 
	{
		_Color ("Main Color", Color) = (0.78, 0.713, 0.505, 1)
		_MainTex ("Texture", 2D) = "" {}
	}
	SubShader
	{
		Tags {"RenderType"="Transparent" "Queue" = "Transparent+1"}

		Stencil {
			Ref 1
			Comp Equal
		}

		Pass
		{
			Cull Back
			ZWrite Off

			//Blend SrcAlpha OneMinusSrcAlpha
			Blend One OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			uniform float4 _Color;

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
				return _Color;
			}
			ENDCG
		}

		Pass
		{
			Cull Back
			ZWrite Off

			//Blend SrcAlpha OneMinusSrcAlpha
			Blend One OneMinusSrcAlpha

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
				return tex2D(_MainTex, input.tex.xy);
			}
			ENDCG
		}
	}
}