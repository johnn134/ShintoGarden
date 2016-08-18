Shader "4DShaders/4D_Render_Order_Shader"
{
	Properties
	{
		_WPos ("W Position", Range(0.0, 6.0)) = 0
		_Threshold ("Shading Threshold", Range(-1.0, 1.0)) = 0.0
		_Dim ("Shading Factor", Range(0.0, 1.0)) = 0.25
	}
	SubShader
	{
		Tags { "Queue" = "Transparent+1" }

		Pass	//Back pass
		{
			Cull Back

			ZWrite Off

			Blend SrcAlpha Zero

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform float4 _LightColor0;

			uniform float _WPos;
			uniform float _Threshold;
			uniform float _Dim;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 posWorld : TEXCOORD1;
				float3 normalDir : TEXCOORD2;
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

				float3 fragmentColor = float4(clamp(0.0, 1.0, ((_WPos + 3) % 6) - 2), 
							   		   		  clamp(0.0, 1.0, ((_WPos + 1) % 6) - 2) + max(0.0, (1 - ((_WPos + 5) % 6))) * 0.5, 
									   		  clamp(0.0, 1.0, _WPos - 3), 
							   		   		  1.0);
				//Cel Shading
				if(attenuation * dot(normalDirection, lightDirection) <= _Threshold)
				{
					fragmentColor *= _Dim;
				}

				return float4(fragmentColor, 1.0);
			}
			ENDCG
		}
	}
}
