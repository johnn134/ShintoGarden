Shader "Cg_Tutorial/Smooth_Specular_Highlights_Shader"
{
	Properties
	{
		_Color ("Diffuse Material Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_SpecColor ("Specular Material Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Shininess ("Shininess", Float) = 10
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
			uniform float4 _SpecColor;
			uniform float _Shininess;

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
				float3x3 modelMatrixInverse = unity_WorldToObject;

				o.posWorld = mul(modelMatrix, input.vertex);
				o.normalDir = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
				o.pos = mul(UNITY_MATRIX_MVP, input.vertex);

				return o;
			}
			
			float4 frag (vertexOutput i) : COLOR
			{
				float3 normalDirection = normalize(i.normalDir);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - i.posWorld.xyz);
				float3 lightDirection;
				float attenuation;

				//Diffuse Lighting
				if(_WorldSpaceLightPos0.w == 0.0)	//directional light
				{
					attenuation = 1.0;
					lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				}
				else	//point or spot light
				{
					float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - i.posWorld.xyz;
					float distance = length(vertexToLightSource);
					attenuation = 1.0 / distance;
					lightDirection = normalize(vertexToLightSource);
				}

				float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb * _Color.rgb;

				float3 diffuseReflection = attenuation * _LightColor0.rgb
				 							* _Color.rgb * max(0.0, dot(normalDirection, lightDirection));

				//Specular Reflection
				float3 specularReflection;
				if(dot(normalDirection, lightDirection) < 0.0)	//wrong side
				{
					specularReflection = float3(0.0, 0.0, 0.0);
				}
				else	//right side
				{
					specularReflection = attenuation * _LightColor0.rgb * _SpecColor.rgb * 
											pow(max(0.0, dot(reflect(-lightDirection, normalDirection), 
															 viewDirection)), _Shininess);
				}

				return float4(ambientLighting + diffuseReflection + specularReflection, 1.0);
			}
			ENDCG
		}
		Pass
		{
			Tags { "LightMode"="ForwardAdd" }

			Blend One One

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform float4 _LightColor0;
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;

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
				float3x3 modelMatrixInverse = unity_WorldToObject;

				o.posWorld = mul(modelMatrix, input.vertex);
				o.normalDir = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
				o.pos = mul(UNITY_MATRIX_MVP, input.vertex);

				return o;
			}
			
			float4 frag (vertexOutput i) : COLOR
			{
				float3 normalDirection = normalize(i.normalDir);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - i.posWorld.xyz);
				float3 lightDirection;
				float attenuation;

				//Diffuse Lighting
				if(_WorldSpaceLightPos0.w == 0.0)	//directional light
				{
					attenuation = 1.0;
					lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				}
				else	//point or spot light
				{
					float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - i.posWorld.xyz;
					float distance = length(vertexToLightSource);
					attenuation = 1.0 / distance;
					lightDirection = normalize(vertexToLightSource);
				}

				float3 diffuseReflection = attenuation * _LightColor0.rgb
				 							* _Color.rgb * max(0.0, dot(normalDirection, lightDirection));

				//Specular Reflection
				float3 specularReflection;
				if(dot(normalDirection, lightDirection) < 0.0)	//wrong side
				{
					specularReflection = float3(0.0, 0.0, 0.0);
				}
				else	//right side
				{
					specularReflection = attenuation * _LightColor0.rgb * _SpecColor.rgb * 
											pow(max(0.0, dot(reflect(-lightDirection, normalDirection), 
															 viewDirection)), _Shininess);
				}

				return float4(diffuseReflection + specularReflection, 1.0);
			}
			ENDCG
		}
	}
	Fallback "Specular"
}

