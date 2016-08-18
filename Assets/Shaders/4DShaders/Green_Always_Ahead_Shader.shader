Shader "4DShaders/Green_Always_Ahead_Shader"
{
	Properties
	{
		_Alpha ("Alpha Value", Range(0.0, 1.0)) = 0.5
		_Shadow ("Shading Weight", Range(0.0, 1.0)) = 1.0
	}
	SubShader
	{
		Tags { "Queue" = "Transparent+1" }

		Pass	//Back pass
		{
			Cull Front

			ZWrite Off

			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform float4 _LightColor0;

			uniform float _Alpha;
			uniform float _Shadow;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float3 col : COLOR;
			};

			vertexOutput vert (vertexInput input)
			{
				vertexOutput o;

				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;

				float3 normalDirection = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);

				float3 lightReflection = _LightColor0.rgb; 
				float lambertWeight = max(0.0, dot(normalDirection, lightDirection));

				o.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				o.col = lightReflection * lambertWeight * _Shadow;
					
				return o;
			}

			float4 frag (vertexOutput input) : COLOR
			{
				return float4(0.0, 1.0, 0.0, _Alpha);
			}
			ENDCG
		}

		Pass	//Front pass
		{
			Cull Back

			ZWrite Off

			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform float4 _LightColor0;

			uniform float _Alpha;
			uniform float _Shadow;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float3 col : COLOR;
			};

			vertexOutput vert (vertexInput input)
			{
				vertexOutput o;

				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;

				float3 normalDirection = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);

				float3 lightReflection = _LightColor0.rgb; 
				float lambertWeight = max(0.0, dot(normalDirection, lightDirection));

				o.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				o.col = lightReflection * lambertWeight * _Shadow;
					
				return o;
			}

			float4 frag (vertexOutput input) : COLOR
			{
				return float4(0.0, 1.0, 0.0, _Alpha) * float4(input.col, 1.0);
			}
			ENDCG
		}
	}
}