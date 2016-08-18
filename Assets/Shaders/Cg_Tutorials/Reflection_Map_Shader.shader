// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Cg_Tutorial/Reflection_Map_Shader"
{
	Properties
	{
		_Cube ("Reflection Map", Cube) = "" {}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform samplerCUBE _Cube;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float3 normalDir : TEXCOORD0;
				float3 viewDir : TEXCOORD1;
			};
			
			vertexOutput vert (vertexInput input)
			{
				vertexOutput o;

				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;

				o.viewDir = mul(modelMatrix, input.vertex).xyz - _WorldSpaceCameraPos;
				o.normalDir = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
				o.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				return o;
			}
			
			fixed4 frag (vertexOutput input) : SV_Target
			{
				float3 reflectedDir = reflect(input.viewDir, normalize(input.normalDir));
				return texCUBE(_Cube, reflectedDir);
			}
			ENDCG
		}
	}
}
