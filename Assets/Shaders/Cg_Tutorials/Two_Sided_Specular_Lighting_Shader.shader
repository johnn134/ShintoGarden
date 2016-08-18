Shader "Cg_Tutorial/Two_Sided_Specular_Lighting_Shader"
{
	Properties
	{
		_Color ("Front Material Diffuse Color", Color) = (1,1,1,1)
		_SpecColor ("Front Material Specular Color", Color) = (1,1,1,1)
		_Shininess ("Front Material Shininess", Float) = 10
		_BackColor ("Back Material Diffuse Color", Color) = (1,1,1,1)
		_BackSpecColor ("Back Material Specular Color", Color) = (1,1,1,1)
		_BackShininess ("Back Material Shininess", Float) = 10
	}
	SubShader
	{
		//Front
		Pass	//Diffuse and Specular Material
		{
			Tags { "LightMode"="ForwardBase" }

			Cull Back

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform float4 _LightColor0;
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;
			uniform float4 _BackColor;
			uniform float4 _BackSpecColor;
			uniform float _BackShininess;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 posInObjectCoords : TEXCOORD;
				float4 col : COLOR;
			};
			
			vertexOutput vert (vertexInput input)
			{
				vertexOutput o;

				float4x4 modelMatrix = unity_ObjectToWorld;
				float3x3 modelMatrixInverse = unity_WorldToObject;

				float3 normalDirection = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - mul(modelMatrix, input.vertex).xyz);
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
					float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - mul(modelMatrix, input.vertex).xyz;
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

				o.col = float4(ambientLighting + diffuseReflection + specularReflection, 1.0);
				o.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				o.posInObjectCoords = input.vertex;

				return o;
			}
			
			float4 frag (vertexOutput i) : COLOR
			{
				if(i.posInObjectCoords.y > 0.0) {
					discard;
				}
				return i.col;
			}
			ENDCG
		}
		Pass	//Specular Highlighting
		{
			Tags { "LightMode"="ForwardAdd" }

			Blend One One
			Cull Back

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform float4 _LightColor0;
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;
			uniform float4 _BackColor;
			uniform float4 _BackSpecColor;
			uniform float _BackShininess;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 posInObjectCoords : TEXCOORD;
				float4 col : COLOR;
			};
			
			vertexOutput vert (vertexInput input)
			{
				vertexOutput o;

				float4x4 modelMatrix = unity_ObjectToWorld;
				float3x3 modelMatrixInverse = unity_WorldToObject;

				float3 normalDirection = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - mul(modelMatrix, input.vertex).xyz);
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
					float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - mul(modelMatrix, input.vertex).xyz;
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

				o.col = float4(diffuseReflection + specularReflection, 1.0);
				o.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				o.posInObjectCoords = input.vertex;

				return o;
			}
			
			float4 frag (vertexOutput i) : COLOR
			{
				if(i.posInObjectCoords.y > 0.0) {
					discard;
				}
				return i.col;
			}
			ENDCG
		}

		//Back
		Pass	//Diffuse and Specular Material
		{
			Tags { "LightMode"="ForwardBase" }

			Cull Front

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform float4 _LightColor0;
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;
			uniform float4 _BackColor;
			uniform float4 _BackSpecColor;
			uniform float _BackShininess;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 posInObjectCoords : TEXCOORD;
				float4 col : COLOR;
			};
			
			vertexOutput vert (vertexInput input)
			{
				vertexOutput o;

				float4x4 modelMatrix = unity_ObjectToWorld;
				float3x3 modelMatrixInverse = unity_WorldToObject;

				float3 normalDirection = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - mul(modelMatrix, input.vertex).xyz);
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
					float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - mul(modelMatrix, input.vertex).xyz;
					float distance = length(vertexToLightSource);
					attenuation = 1.0 / distance;
					lightDirection = normalize(vertexToLightSource);
				}

				float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb * _BackColor.rgb;

				float3 diffuseReflection = attenuation * _LightColor0.rgb
				 							* _BackColor.rgb * max(0.0, dot(normalDirection, lightDirection));

				//Specular Reflection
				float3 specularReflection;
				if(dot(normalDirection, lightDirection) < 0.0)	//wrong side
				{
					specularReflection = float3(0.0, 0.0, 0.0);
				}
				else	//right side
				{
					specularReflection = attenuation * _LightColor0.rgb * _BackSpecColor.rgb * 
											pow(max(0.0, dot(reflect(-lightDirection, normalDirection), 
															 viewDirection)), _BackShininess);
				}

				o.col = float4(ambientLighting + diffuseReflection + specularReflection, 1.0);
				o.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				o.posInObjectCoords = input.vertex;

				return o;
			}
			
			float4 frag (vertexOutput i) : COLOR
			{
				if(i.posInObjectCoords.y > 0.0) {
					discard;
				}
				return i.col;
			}
			ENDCG
		}
		Pass	//Specular Highlighting
		{
			Tags { "LightMode"="ForwardAdd" }

			Blend One One
			Cull Back

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform float4 _LightColor0;
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;
			uniform float4 _BackColor;
			uniform float4 _BackSpecColor;
			uniform float _BackShininess;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 posInObjectCoords : TEXCOORD;
				float4 col : COLOR;
			};
			
			vertexOutput vert (vertexInput input)
			{
				vertexOutput o;

				float4x4 modelMatrix = unity_ObjectToWorld;
				float3x3 modelMatrixInverse = unity_WorldToObject;

				float3 normalDirection = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - mul(modelMatrix, input.vertex).xyz);
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
					float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - mul(modelMatrix, input.vertex).xyz;
					float distance = length(vertexToLightSource);
					attenuation = 1.0 / distance;
					lightDirection = normalize(vertexToLightSource);
				}

				float3 diffuseReflection = attenuation * _LightColor0.rgb
				 							* _BackColor.rgb * max(0.0, dot(normalDirection, lightDirection));

				//Specular Reflection
				float3 specularReflection;
				if(dot(normalDirection, lightDirection) < 0.0)	//wrong side
				{
					specularReflection = float3(0.0, 0.0, 0.0);
				}
				else	//right side
				{
					specularReflection = attenuation * _LightColor0.rgb * _BackSpecColor.rgb * 
											pow(max(0.0, dot(reflect(-lightDirection, normalDirection), 
															 viewDirection)), _BackShininess);
				}

				o.col = float4(diffuseReflection + specularReflection, 1.0);
				o.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				o.posInObjectCoords = input.vertex;

				return o;
			}
			
			float4 frag (vertexOutput i) : COLOR
			{
				if(i.posInObjectCoords.y > 0.0) {
					discard;
				}
				return i.col;
			}
			ENDCG
		}
	}
	FallBack "Specular"
}
