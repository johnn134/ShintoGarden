Shader "Cg_Tutorial/Toon_Shading_Shader"
{
	Properties
	{
		_Color ("Diffuse Color", Color) = (1,1,1,1)
		_UnlitColor ("Unlit Diffuse Color", Color) = (0.5,0.5,0.5,1)
		_DiffuseThreshold ("Threshold for Diffuse Colors", Range(0,1)) = 0.1
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_LitOutlineThickness ("Lit Outline Thickness", Range(0,1)) = 0.1
		_UnlitOutlineThickness ("Unlit Outline Thickness", Range(0,1)) = 0.4
		_SpecColor ("Specular Color", Color) = (1,1,1,1)
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
			uniform float4 _UnlitColor;
			uniform float _DiffuseThreshold;
			uniform float4 _OutlineColor;
			uniform float _LitOutlineThickness;
			uniform float _UnlitOutlineThickness;
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
				float3 fragmentColor = _UnlitColor.rgb;

				//low priority: diffuse illumination
				if(attenuation * max(0.0, dot(normalDirection, lightDirection)) >= _DiffuseThreshold)
				{
					fragmentColor = _LightColor0.rgb * _Color.rgb;
				}

				//higher priority: outline
				if(dot(viewDirection, normalDirection)
					< lerp(_UnlitOutlineThickness, _LitOutlineThickness, max(0.0, dot(normalDirection, lightDirection))))
				{
					fragmentColor = _LightColor0.rgb * _OutlineColor.rgb;
				}

				//highest priority: highlights
				if(dot(normalDirection, lightDirection) > 0.0
				   && attenuation * pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shininess) > 0.5)
				{
					fragmentColor = _SpecColor.a * _LightColor0.rgb
								    * _SpecColor.rgb + (1.0 - _SpecColor.a) * fragmentColor;
				}

				return float4(fragmentColor, 1.0);
			}
			ENDCG
		}

		Pass
		{
			Tags { "LightMode"="ForwardAdd" }

			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform float4 _LightColor0;
			uniform float4 _Color;
			uniform float4 _UnlitColor;
			uniform float _DiffuseThreshold;
			uniform float4 _OutlineColor;
			uniform float _LitOutlineThickness;
			uniform float _UnlitOutlineThickness;
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

				float4 fragmentColor = float4(0.0, 0.0, 0.0, 0.0);

				if(dot(normalDirection, lightDirection) > 0.0
				   && attenuation * pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shininess) > 0.5)
				{
					fragmentColor = float4(_LightColor0.rgb, 1.0) * _SpecColor;
				}

				return fragmentColor;
			}
			ENDCG
		}
	}
	Fallback "Specular"
}
