﻿// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Cg_Tutorial/Hemisphere_Per_Vertex_Lighting_Shader"
{
	Properties
	{
		_Color ("Diffuse Material Color", Color) = (1,1,1,1)
		_UpperHemisphereColor ("Upper Hemisphere Color", Color) = (1,1,1,1)
		_LowerHemisphereColor ("Lower Hemisphere Color", Color) = (1,1,1,1)
		_UpVector ("Up Vector", Vector) = (0,1,0,0)
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform float4 _Color;
			uniform float4 _UpperHemisphereColor;
			uniform float4 _LowerHemisphereColor;
			uniform float4 _UpVector; 

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 col : COLOR;
			};
			
			vertexOutput vert (vertexInput input)
			{
				vertexOutput o;

				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;

				float3 normalDirection = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
				float3 upDirection = normalize(_UpVector);

				float w = 0.5 * (1.0 + dot(upDirection, normalDirection));

				o.col = (w * _UpperHemisphereColor + (1.0 - w) * _LowerHemisphereColor) * _Color;
				o.pos = mul(UNITY_MATRIX_MVP, input.vertex);

				return o;
			}
			
			float4 frag (vertexOutput input) : COLOR
			{
				return input.col;
			}
			ENDCG
		}
	}
}
