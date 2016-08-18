Shader "Cg_Tutorial/Fresnel_Highlights_Shader"
{
	Properties
	{
		_Color ("Diffuse Material Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Material Color", Color) = (1,1,1,1)
		_Shininess ("Shininess", Float) = 10
		_Fresnel ("Fresnel Factor", Range(0.0, 10.0)) = 5.0
	}
	SubShader
	{
		Pass
		{
			Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform float4 _LightColor0;
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;
			uniform float _Fresnel;

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
				o.normalDir = normalize(mul(input.normal, modelMatrixInverse));
				o.pos = mul(UNITY_MATRIX_MVP, input.vertex);
					
				return o;
			}
			
			float4 frag (vertexOutput input) : COLOR
			{
				float3 normalDirection = normalize(input.normalDir);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);
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
					float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
					float distance = length(vertexToLightSource);
					attenuation = 1.0 / distance;
					lightDirection = normalize(vertexToLightSource);
				}

				float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb * _Color.rgb;

				float3 diffuseReflection = attenuation * _LightColor0.rgb * _Color.rgb
										   * max(0.0, dot(normalDirection, lightDirection));

				//Specular Reflection
				float3 specularReflection;
				if(dot(normalDirection, lightDirection) < 0.0)	//wrong side
				{
					specularReflection = float3(0.0, 0.0, 0.0);
				}
				else	//right side
				{
					float3 halfwayDirection = normalize(lightDirection + viewDirection);

					float w = pow(1.0 - max(0.0, dot(halfwayDirection, viewDirection)), _Fresnel);

					specularReflection = attenuation * _LightColor0.rgb
										 * lerp(_SpecColor.rgb, float3(1.0, 1.0, 1.0), w)
										 * pow(max(0.0, dot(reflect(-lightDirection, normalDirection), 
										 		   					viewDirection)), _Shininess);
				}	

				return float4(ambientLighting + diffuseReflection + specularReflection, 1.0);
			}
			ENDCG	
		}

		Pass
		{
			Tags { "LightMode" = "ForwardAdd" }

			Blend One One

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform float4 _LightColor0;
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;
			uniform float _Fresnel;

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
				o.normalDir = normalize(mul(input.normal, modelMatrixInverse));
				o.pos = mul(UNITY_MATRIX_MVP, input.vertex);
					
				return o;
			}
			
			float4 frag (vertexOutput input) : COLOR
			{
				float3 normalDirection = normalize(input.normalDir);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);
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
					float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
					float distance = length(vertexToLightSource);
					attenuation = 1.0 / distance;
					lightDirection = normalize(vertexToLightSource);
				}

				float3 diffuseReflection = attenuation * _LightColor0.rgb * _Color.rgb
										   * max(0.0, dot(normalDirection, lightDirection));

				//Specular Reflection
				float3 specularReflection;
				if(dot(normalDirection, lightDirection) < 0.0)	//wrong side
				{
					specularReflection = float3(0.0, 0.0, 0.0);
				}
				else	//right side
				{
					float3 halfwayDirection = normalize(lightDirection + viewDirection);

					float w = pow(1.0 - max(0.0, dot(halfwayDirection, viewDirection)), _Fresnel);

					specularReflection = attenuation * _LightColor0.rgb
										 * lerp(_SpecColor.rgb, float3(1.0, 1.0, 1.0), w)
										 * pow(max(0.0, dot(reflect(-lightDirection, normalDirection), 
										 		   					viewDirection)), _Shininess);
				}

				return float4(diffuseReflection + specularReflection, 1.0);
			}
			ENDCG
		}
	}
	Fallback "Specular"
}