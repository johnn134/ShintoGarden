// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Cg_Tutorial/Multitexturing_Shader"
{
	Properties
	{
		_DecalTex ("Daytime Earth", 2D) = "white" {}
		_MainTex ("Nighttime Earth", 2D) = "white" {}
		_Color ("Nighttime Color Filter", Color) = (1,1,1,1)
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
			uniform sampler2D _DecalTex;
			uniform float4 _Color;

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
				float levelOfLighting : TEXCOORD1;
			};
			
			vertexOutput vert (vertexInput input)
			{
				vertexOutput o;

				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;

				float3 normalDirection = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);

				o.levelOfLighting = max(0.0, dot(normalDirection, lightDirection));
				o.tex = input.texcoord;
				o.pos = mul(UNITY_MATRIX_MVP, input.vertex);

				return o;
			}
			
			fixed4 frag (vertexOutput input) : COLOR
			{
				float4 nighttimeColor = tex2D(_MainTex, input.tex.xy);
				float4 daytimeColor = tex2D(_DecalTex, input.tex.xy);
				return lerp(nighttimeColor, daytimeColor, input.levelOfLighting);
			}
			ENDCG
		}
	}
	Fallback "Decal"
}
