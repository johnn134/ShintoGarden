Shader "Additional_Testing/Cel_Shading_Lambertian_Shader"
{
	Properties
	{
		_Color ("Diffuse Material Color", Color) = (1,1,1,1)
		_BandWidth ("Width of Cel Band", Range (0.0, 1.0)) = 0.05
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

			uniform float4 _LightColor0;
			uniform float4 _Color;
			uniform float _BandWidth;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 posWorld : TEXCOORD0;
				float3 normalDir : TEXCOORD1;
			};

			vertexOutput vert (vertexInput input)
			{
				vertexOutput o;

				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;

				o.posWorld = mul(modelMatrix, input.vertex);
				o.normalDir = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
				o.pos = mul(UNITY_MATRIX_MVP, input.vertex);
					
				return o;
			}

			float4 frag (vertexOutput input) : COLOR
			{
				float3 normalDirection = normalize(input.normalDir);
				//float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);
				float3 lightDirection;
				float attenuation;

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

				//default: unlit
				float3 fragmentColor = float3(0.0,0.0,0.0);

				for(int index = 1; index < 4; index++)
				{
					float angle = attenuation * max(0.0, dot(normalDirection, lightDirection));

					if(angle >= index * _BandWidth)
					{
						fragmentColor = _Color.rgb * (index / 3.0);
					}
				}

				return float4(fragmentColor, 1.0);
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
