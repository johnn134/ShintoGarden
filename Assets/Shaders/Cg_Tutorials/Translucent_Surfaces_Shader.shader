Shader "Cg_Tutorial/Translucent_Surfaces_Shader"
{
	Properties
	{
		_Color ("Diffuse Material Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Material Color", Color) = (1,1,1,1)
		_Shininess ("Shininess", Float) = 10
		_DiffuseTranslucentColor ("Diffuse Translucent Color", Color) = (1,1,1,1)
		_ForwardTranslucentColor ("Forward Translucent Color", Color) = (1,1,1,1)
		_Sharpness ("Sharpness", Float) = 10
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
			uniform float4 _DiffuseTranslucentColor;
			uniform float4 _ForwardTranslucentColor;
			uniform float _Sharpness;

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
				float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);

				normalDirection = faceforward(normalDirection, -viewDirection, normalDirection);

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

				float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb * _Color.rgb;

				float3 diffuseReflection = attenuation * _LightColor0.rgb * _Color.rgb
										   * max(0.0, dot(normalDirection, lightDirection));

				float3 specularReflection;
				if(dot(normalDirection, lightDirection) < 0.0)
				{
					specularReflection = float3(0.0, 0.0, 0.0);
				}
				else
				{
					specularReflection = attenuation * _LightColor0.rgb * _SpecColor.rgb
										 * pow(max(0.0, dot(reflect(-lightDirection, normalDirection),
										 					viewDirection)), _Shininess);
				}

				float3 diffuseTranslucency = attenuation * _LightColor0.rgb * _DiffuseTranslucentColor.rgb
											 * max(0.0, dot(lightDirection, -normalDirection));

				float3 forwardTranslucency;
				if(dot(normalDirection, lightDirection) > 0.0)
				{
					forwardTranslucency = float3(0.0, 0.0, 0.0);
				}
				else
				{
					forwardTranslucency = attenuation * _LightColor0.rgb * _ForwardTranslucentColor.rgb
										  * pow(max(0.0, dot(-lightDirection, viewDirection)), _Sharpness);
				}

				return float4(ambientLighting + diffuseReflection + specularReflection
							  + diffuseTranslucency + forwardTranslucency, 1.0);
			}
			ENDCG
		}

		Pass
		{
			Tags { "LightMode"="ForwardAdd" }

			Cull Off
			Blend One One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform float4 _LightColor0;
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;
			uniform float4 _DiffuseTranslucentColor;
			uniform float4 _ForwardTranslucentColor;
			uniform float _Sharpness;

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
				float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);

				normalDirection = faceforward(normalDirection, -viewDirection, normalDirection);

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

				float3 diffuseReflection = attenuation * _LightColor0.rgb * _Color.rgb
										   * max(0.0, dot(normalDirection, lightDirection));

				float3 specularReflection;
				if(dot(normalDirection, lightDirection) < 0.0)
				{
					specularReflection = float3(0.0, 0.0, 0.0);
				}
				else
				{
					specularReflection = attenuation * _LightColor0.rgb * _SpecColor.rgb
										 * pow(max(0.0, dot(reflect(-lightDirection, normalDirection),
										 					viewDirection)), _Shininess);
				}

				float3 diffuseTranslucency = attenuation * _LightColor0.rgb * _DiffuseTranslucentColor.rgb
											 * max(0.0, dot(lightDirection, -normalDirection));

				float3 forwardTranslucency;
				if(dot(normalDirection, lightDirection) > 0.0)
				{
					forwardTranslucency = float3(0.0, 0.0, 0.0);
				}
				else
				{
					forwardTranslucency = attenuation * _LightColor0.rgb * _ForwardTranslucentColor.rgb
										  * pow(max(0.0, dot(-lightDirection, viewDirection)), _Sharpness);
				}

				return float4(diffuseReflection + specularReflection
							  + diffuseTranslucency + forwardTranslucency, 1.0);
			}
			ENDCG
		}
	}
}
