Shader "Cg_Tutorial/Better_Cutaway_Shader"
{
	SubShader 
   	{
   		Pass 
      	{
        	Cull Off // turn off triangle culling, alternatives are:
	         // Cull Back (or nothing): cull only back faces 
	         // Cull Front : cull only front faces
	 
	         CGPROGRAM 
	 
	         #pragma vertex vert  
	         #pragma fragment frag 

			 #include "UnityCG.cginc"

			 uniform float4x4 _Cutaway;
	 
	         struct vertexInput {
	            float4 vertex : POSITION;
	         };
	         struct vertexOutput {
	            float4 pos : SV_POSITION;
	            float4 posInWorldCoords : TEXCOORD0;
	            float4 cutawayPos : TEXCOORD1;
	         };
	 
	         vertexOutput vert(float4 vertex : POSITION) 
	         {
	            vertexOutput o;
	 
	            o.pos =  mul(UNITY_MATRIX_MVP, vertex);
	            o.posInWorldCoords = mul(unity_ObjectToWorld, vertex);
	            o.cutawayPos = mul(_Cutaway, o.posInWorldCoords);
	 
	            return o;
	         }
	 
	         float4 frag(vertexOutput i) : COLOR 
	         {
	            if (dot(i.cutawayPos.xyz, i.cutawayPos.xyz) < 0.25) 
	            {
	               discard; // drop the fragment if y coordinate > 0
	            }
	            return float4(0.0, 1.0, 0.0, 1.0); // green
	         }
	 
	         ENDCG  
      	}
   	}
}
