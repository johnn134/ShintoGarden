Shader "Unlit/Test_AlwaysShow"
{
	Properties
	{
		_Color("Main Color", Color) = (1, 1, 1, 1)
		_Alpha("Alpha", Range(0.0, 1.0)) = 0.5
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "LightMode"="ForwardBase"}
		Lighting On 
		Cull Back
		ZTest Always
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

			fixed4 _Color;
			float _Alpha;

			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color : TEXCOORD0;
				fixed4 diff : COLOR0;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color.xyz = _Color.rgb;
				o.color.w = _Alpha;

				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				half nl = max(0.5, dot(worldNormal, _WorldSpaceLightPos0.xyz));

				o.diff = nl * _LightColor0;
				o.diff.rgb += ShadeSH9(half4(worldNormal, 1));

				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				return i.color * i.diff;
			}
			ENDCG
		}
	}
}
