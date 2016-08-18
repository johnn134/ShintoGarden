// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Cg_Tutorial/Silhouette_Enhancement_Shader"
{
	Properties
	{
		_Color ("Color", Color) = (1.0, 1.0, 1.0, 0.5)
		_Thickness ("Thickness", Float) = 1.0
	}
	SubShader
	{
		Tags { "Queue"="Transparent" }

		Pass
		{
			ZWrite Off

			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 normal : TEXCOORD;
				float3 viewDir : TEXCOORD1;
			};

			uniform float4 _Color;
			uniform float _Thickness;
			
			v2f vert (appdata v)
			{
				v2f o;

				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

				o.normal = normalize(mul(float4(v.normal, 0.0), modelMatrixInverse).xyz);
				o.viewDir = normalize(_WorldSpaceCameraPos - mul(modelMatrix, v.vertex).xyz);

				return o;
			}
			
			float4 frag (v2f i) : COLOR
			{
				float3 normalDirection = normalize(i.normal);
				float3 viewDirection = normalize(i.viewDir);

				float newOpacity = min(1.0, _Color.a / pow(abs(dot(viewDirection, normalDirection)), _Thickness));
				return float4(_Color.rgb, newOpacity);
			}
			ENDCG
		}
	}
}
