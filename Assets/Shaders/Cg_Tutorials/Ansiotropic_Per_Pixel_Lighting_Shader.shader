// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Cg_Tutorial/Ansiotropic_Per_Pixel_Lighting_Shader"
{
	Properties
	{
		_Color ("Diffuse Material Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Material Color", Color) = (1,1,1,1)
		_AlphaX ("Roughness in Brush Direction", Range (0.0, 5.0)) = 1.0
		_AlphaY ("Roughness orthogonal to Brush Direction", Range (0.0, 5.0)) = 1.0
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
			uniform float _AlphaX;
			uniform float _AlphaY;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 posWorld : TEXCOORD0;
				float3 viewDir : TEXCOORD1;
				float3 normalDir : TEXCOORD2;
				float3 tangentDir : TEXCOORD3;
			};
			
			vertexOutput vert (vertexInput input)
			{
				vertexOutput o;

				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;

				o.posWorld = mul(modelMatrix, input.vertex);
				o.viewDir = normalize(_WorldSpaceCameraPos - o.posWorld.xyz);
				o.normalDir = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
				o.tangentDir = normalize(mul(modelMatrix, float4(input.tangent.xyz, 0.0)).xyz);
				o.pos = mul(UNITY_MATRIX_MVP, input.vertex);
					
				return o;
			}
			
			float4 frag (vertexOutput input) : COLOR
			{
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

				float3 halfwayVector = normalize(lightDirection + input.viewDir);
				float3 binormalDirection = cross(input.normalDir, input.tangentDir);
				float dotLN = dot(lightDirection, input.normalDir);

				float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb * _Color.rgb;

				float3 diffuseReflection = attenuation * _LightColor0.rgb * _Color.rgb * max(0.0, dotLN);

				//Specular Reflection
				float3 specularReflection;
				if(dotLN < 0.0)	//wrong side
				{
					specularReflection = float3(0.0, 0.0, 0.0);
				}
				else	//right side
				{
					float dotHN = dot(halfwayVector, input.normalDir);
					float dotVN = dot(input.viewDir, input.normalDir);
					float dotHTAlphaX = dot(halfwayVector, input.tangentDir) / _AlphaX;
					float dotHBAlphaY = dot(halfwayVector, binormalDirection) / _AlphaY;

					specularReflection = attenuation * _LightColor0.rgb * _SpecColor.rgb
										 * sqrt(max(0.0, dotLN / dotVN))
										 * exp(-2.0 * (dotHTAlphaX * dotHTAlphaX + dotHBAlphaY * dotHBAlphaY) / (1.0 + dotHN));
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
			uniform float _AlphaX;
			uniform float _AlphaY;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 posWorld : TEXCOORD0;
				float3 viewDir : TEXCOORD1;
				float3 normalDir : TEXCOORD2;
				float3 tangentDir : TEXCOORD3;
			};
			
			vertexOutput vert (vertexInput input)
			{
				vertexOutput o;

				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;

				o.posWorld = mul(modelMatrix, input.vertex);
				o.viewDir = normalize(_WorldSpaceCameraPos - o.posWorld.xyz);
				o.normalDir = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
				o.tangentDir = normalize(mul(modelMatrix, float4(input.tangent.xyz, 0.0)).xyz);
				o.pos = mul(UNITY_MATRIX_MVP, input.vertex);

				return o;
			}
			
			float4 frag (vertexOutput input) : COLOR
			{
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

				float3 halfwayVector = normalize(lightDirection + input.viewDir);
				float3 binormalDirection = cross(input.normalDir, input.tangentDir);
				float dotLN = dot(lightDirection, input.normalDir);

				float3 diffuseReflection = attenuation * _LightColor0.rgb * _Color.rgb * max(0.0, dotLN);

				//Specular Reflection
				float3 specularReflection;
				if(dotLN < 0.0)	//wrong side
				{
					specularReflection = float3(0.0, 0.0, 0.0);
				}
				else	//right side
				{
					float dotHN = dot(halfwayVector, input.normalDir);
					float dotVN = dot(input.viewDir, input.normalDir);
					float dotHTAlphaX = dot(halfwayVector, input.tangentDir) / _AlphaX;
					float dotHBAlphaY = dot(halfwayVector, binormalDirection) / _AlphaY;

					specularReflection = attenuation * _LightColor0.rgb * _SpecColor.rgb
										 * sqrt(max(0.0, dotLN / dotVN))
										 * exp(-2.0 * (dotHTAlphaX * dotHTAlphaX + dotHBAlphaY * dotHBAlphaY) / (1.0 + dotHN));
				}

				return float4(diffuseReflection + specularReflection, 1.0);
			}
			ENDCG
		}
	}
	Fallback "Specular"
}
