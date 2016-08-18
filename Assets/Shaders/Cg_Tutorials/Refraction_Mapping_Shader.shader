Shader "Cg_Tutorial/Refraction_Mapping_Shader"
{
	Properties
	{
		_Cube ("Reflection Map", Cube) = "" {}
		_RefIndex ("Refraction Index", Range (1.0, 5.0)) = 1.5
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
			uniform float _RefIndex;

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
				float refractiveIndex = _RefIndex;
				float3 reflectedDir = refract(normalize(input.viewDir), 
											  normalize(input.normalDir), 
											  1.0 / refractiveIndex);
				return texCUBE(_Cube, reflectedDir);
			}
			ENDCG
		}
	}
}

