Shader "Cg_Tutorial/Single_Texture_Per_Pixel_Lighting_Shader"
{
	Properties
	{
		_MainTex ("RGBA Texture For material Color", 2D) = "white" {}
		_Color ("Overall Diffuse Color Filter", Color) = (1,1,1,1)
		_SpecColor ("Specular Material Color", Color) = (1,1,1,1)
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
			uniform sampler2D _MainTex;
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 posWorld : TEXCOORD0;
				float3 normalDir : TEXCOORD1;
				float4 tex : TEXCOORD2;
			};
			
			vertexOutput vert (vertexInput input)
			{
				vertexOutput o;

				float4x4 modelMatrix = unity_ObjectToWorld;
				float3x3 modelMatrixInverse = unity_WorldToObject;

				o.posWorld = mul(modelMatrix, input.vertex);
				o.normalDir = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
				o.tex = input.texcoord;
				o.pos = mul(UNITY_MATRIX_MVP, input.vertex);

				return o;
			}
			
			float4 frag (vertexOutput i) : COLOR
			{
				float3 normalDirection = normalize(i.normalDir);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - i.posWorld.xyz);
				float3 lightDirection;
				float attenuation;

				float4 textureColor = tex2D(_MainTex, i.tex.xy);

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

				float3 ambientLighting = textureColor.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb * _Color.rgb;

				float3 diffuseReflection = textureColor.rgb * attenuation * _LightColor0.rgb
				 							* _Color.rgb * max(0.0, dot(normalDirection, lightDirection));

				//Specular Reflection
				float3 specularReflection;
				if(dot(normalDirection, lightDirection) < 0.0)	//wrong side
				{
					specularReflection = float3(0.0, 0.0, 0.0);
				}
				else	//right side
				{
					specularReflection = attenuation * _LightColor0.rgb * _SpecColor.rgb * (1.0 - textureColor.a)
										 * pow(max(0.0, dot(reflect(-lightDirection, normalDirection), 
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
			uniform sampler2D _MainTex;
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 posWorld : TEXCOORD0;
				float3 normalDir : TEXCOORD1;
				float4 tex : TEXCOORD2;
			};
			
			vertexOutput vert (vertexInput input)
			{
				vertexOutput o;

				float4x4 modelMatrix = unity_ObjectToWorld;
				float3x3 modelMatrixInverse = unity_WorldToObject;

				o.posWorld = mul(modelMatrix, input.vertex);
				o.normalDir = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
				o.tex = input.texcoord;
				o.pos = mul(UNITY_MATRIX_MVP, input.vertex);

				return o;
			}
			
			float4 frag (vertexOutput i) : COLOR
			{
				float3 normalDirection = normalize(i.normalDir);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - i.posWorld.xyz);
				float3 lightDirection;
				float attenuation;

				float4 textureColor = tex2D(_MainTex, i.tex.xy);

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

				float3 diffuseReflection = textureColor.rgb * attenuation * _LightColor0.rgb
				 							* _Color.rgb * max(0.0, dot(normalDirection, lightDirection));

				//Specular Reflection
				float3 specularReflection;
				if(dot(normalDirection, lightDirection) < 0.0)	//wrong side
				{
					specularReflection = float3(0.0, 0.0, 0.0);
				}
				else	//right side
				{
					specularReflection = attenuation * _LightColor0.rgb * _SpecColor.rgb * (1.0 - textureColor.a)
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
