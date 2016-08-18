Shader "Cg_Tutorial/Parallax_Mapping_Shader"
{
	Properties
	{
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_ParallaxMap ("Heightmap (in A)", 2D) = "black" {}
		_Parallax ("Max Height", Range (0.0, 0.08)) = 0.01
		_MaxTexCoordOffset ("Max Texture Coordinate Offset", Float) = 0.01
		_Color ("Diffuse Material Filter", Color) = (1,1,1,1)
		_SpecColor ("Specular Material Color", Color) = (1,1,1,1)
		_Shininess ("Shininess", Float) = 10
	}
	CGINCLUDE
	
	#include "UnityCG.cginc"

	uniform float4 _LightColor0;

	uniform sampler2D _BumpMap;
	uniform float4 _BumpMap_ST;
	uniform sampler2D _ParallaxMap;
	uniform float4 _ParallaxMap_ST;
	uniform float _Parallax;
	uniform float _MaxTexCoordOffset;
	uniform float4 _Color;
	uniform float4 _SpecColor;
	uniform float _Shininess;

	struct vertexInput
	{
		float4 vertex : POSITION;
		float4 texcoord : TEXCOORD0;
		float3 normal : NORMAL;
		float4 tangent : TANGENT;
	};

	struct vertexOutput
	{
		float4 pos : SV_POSITION;
		float4 posWorld : TEXCOORD0;
		float4 tex : TEXCOORD1;
		float3 tangentWorld : TEXCOORD2;
		float3 normalWorld : TEXCOORD3;
		float3 binormalWorld : TEXCOORD4;
		float3 viewDirWorld : TEXCOORD5;
		float3 viewDirInScaledSurfaceCoords : TEXCOORD6;
	};
	
	vertexOutput vert (vertexInput input)
	{
		vertexOutput o;

		float4x4 modelMatrix = unity_ObjectToWorld;
		float3x3 modelMatrixInverse = unity_WorldToObject;

		o.tangentWorld = normalize(mul(modelMatrix, float4(input.tangent.xyz, 0.0)).xyz);
		o.normalWorld = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
		o.binormalWorld = normalize(cross(o.normalWorld, o.tangentWorld) * input.tangent.w);

		float3 binormal = cross(input.normal, input.tangent.xyz) * input.tangent.w;

		float3 viewDirInObjectCoords = mul(modelMatrixInverse, float4(_WorldSpaceCameraPos, 1.0)).xyz
										 - input.vertex.xyz;

		//vectors are orthoganol
		float3x3 localSurface2ScaledObjectT = float3x3(input.tangent.xyz, binormal, input.normal);

		//we multiply with the transpose to multiply with
		//the inverse (apart from the scaling)
		o.viewDirInScaledSurfaceCoords = mul(localSurface2ScaledObjectT, viewDirInObjectCoords);

		o.posWorld = mul(modelMatrix, input.vertex);
		o.viewDirWorld = normalize(_WorldSpaceCameraPos - o.posWorld.xyz);
		o.tex = input.texcoord;
		o.pos = mul(UNITY_MATRIX_MVP, input.vertex);

		return o;
	}
	
	float4 fragWithAmbient (vertexOutput input) : COLOR
	{
		//Parallax mapping
		float height = _Parallax
					   * (-0.5 + tex2D(_ParallaxMap, _ParallaxMap_ST.xy
					   * input.tex.xy + _ParallaxMap_ST.zw).x);

		float2 texCoordOffsets = clamp(height * input.viewDirInScaledSurfaceCoords.xy
										/ input.viewDirInScaledSurfaceCoords.z,
										-_MaxTexCoordOffset, +_MaxTexCoordOffset);

		//Normal Mapping
		float4 encodedNormal = tex2D(_BumpMap, _BumpMap_ST.xy * (input.tex.xy + texCoordOffsets) + _BumpMap_ST.zw);
		float3 localCoords = float3(2.0 * encodedNormal.a - 1.0, 2.0 * encodedNormal.g - 1.0, 0.0);
		localCoords.z = sqrt(1.0 - dot(localCoords, localCoords));
		//approximation without sqrt: 
		//localCoords.z = 1.0 - 0.5 * dot(localCoords, localCoords);

		float3x3 local2WorldTranspose = float3x3(input.tangentWorld, 
												input.binormalWorld, 
												input.normalWorld);
		float3 normalDirection = normalize(mul(localCoords, local2WorldTranspose));

		float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);
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
			float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
			float distance = length(vertexToLightSource);
			attenuation = 1.0 / distance;
			lightDirection = normalize(vertexToLightSource);
		}

		float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb * _Color.rgb;

		float3 diffuseReflection = attenuation * _LightColor0.rgb * _Color.rgb 
									* max(0.0, dot(normalDirection, lightDirection));

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
		return float4(ambientLighting + diffuseReflection + specularReflection, 1.0);
	}

	float4 fragWithoutAmbient (vertexOutput input) : COLOR
	{
		//Parallax mapping
		float height = _Parallax
					   * (-0.5 + tex2D(_ParallaxMap, _ParallaxMap_ST.xy
					   * input.tex.xy + _ParallaxMap_ST.zw).x);

		float2 texCoordOffsets = clamp(height * input.viewDirInScaledSurfaceCoords.xy
										/ input.viewDirInScaledSurfaceCoords.z,
										-_MaxTexCoordOffset, +_MaxTexCoordOffset);

		//Normal Mapping
		float4 encodedNormal = tex2D(_BumpMap, _BumpMap_ST.xy * (input.tex.xy + texCoordOffsets) + _BumpMap_ST.zw);
		float3 localCoords = float3(2.0 * encodedNormal.a - 1.0, 2.0 * encodedNormal.g - 1.0, 0.0);
		localCoords.z = sqrt(1.0 - dot(localCoords, localCoords));
		//approximation without sqrt: 
		//localCoords.z = 1.0 - 0.5 * dot(localCoords, localCoords);

		float3x3 local2WorldTranspose = float3x3(input.tangentWorld, 
												input.binormalWorld, 
												input.normalWorld);
		float3 normalDirection = normalize(mul(localCoords, local2WorldTranspose));

		float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);
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
			float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
			float distance = length(vertexToLightSource);
			attenuation = 1.0 / distance;
			lightDirection = normalize(vertexToLightSource);
		}

		float3 diffuseReflection = attenuation * _LightColor0.rgb * _Color.rgb 
									* max(0.0, dot(normalDirection, lightDirection));

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
		return float4(diffuseReflection + specularReflection, 1.0);
	}

	ENDCG

	SubShader
	{
		Pass
		{
			Tags { "LightMode"="ForwardBase" }

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment fragWithAmbient
			//the functions are defined in the CGINCLUDE part

			ENDCG
		}

		Pass
		{
			Tags { "LightMode"="ForwardAdd" }

			Blend One One

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment fragWithoutAmbient
			//the functions are defined in the CGINCLUDE part

			ENDCG
		}
	}
}
