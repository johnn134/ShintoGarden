Shader "Additional_Testing/WindWaker_Shader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Diffuse Material Color", Color) = (1,1,1,1)
		_Threshold ("Shading Threshold", Range(-1.0, 1.0)) = 0.0
		_Dim ("Shading Factor", Range(0.0, 1.0)) = 0.25
	}
	SubShader
	{
		Pass
		{
			Tags { "LightMode"="ForwardBase" }

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			//uniform float4 _LightColor0;

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _Color;
			uniform float _Threshold;
			uniform float _Dim;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
				float4 posWorld : TEXCOORD1;
				float3 normalDir : TEXCOORD2;
			};

			vertexOutput vert (vertexInput input)
			{
				vertexOutput o;

				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;

				o.posWorld = mul(modelMatrix, input.vertex);
				o.normalDir = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
				o.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				o.tex = input.texcoord;
					
				return o;
			}

			float4 frag (vertexOutput input) : COLOR
			{
				float3 normalDirection = normalize(input.normalDir);
				float3 lightDirection;
				float attenuation;

				float4 textureColor = tex2D(_MainTex, input.tex.xy);

				if(_WorldSpaceLightPos0.w == 0.0)
				{
					attenuation = 1.0;
					lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				}
				else
				{
					float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
					float distance = length(vertexToLightSource);
					attenuation = 1.0 / distance;
					lightDirection = normalize(vertexToLightSource);
				}

				float3 fragmentColor = _Color.rgb;
				if(attenuation * dot(normalDirection, lightDirection) <= _Threshold)
				{
					fragmentColor = _Color.rgb * _Dim;
				}

				return textureColor * float4(fragmentColor, 1.0);
			}
			ENDCG
		}

	}
	Fallback "Diffuse"
}
