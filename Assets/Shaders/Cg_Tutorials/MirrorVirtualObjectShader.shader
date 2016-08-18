// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Cg_Tutorial/MirrorVirtualObjectShader"
{
   Properties {
      _Color ("Virtual Object's Color", Color) = (1, 1, 1, 1) 
   } 
   SubShader {
      Tags { "Queue" = "Transparent+20" } 
         // render after mirror has been rendered
      
      Pass { 
         Blend OneMinusDstAlpha DstAlpha 
            // when the framebuffer has alpha = 1, keep its color
            // only write color where the framebuffer has alpha = 0

         CGPROGRAM
 
         #pragma vertex vert 
         #pragma fragment frag

         #include "UnityCG.cginc"

         // User-specified uniforms
         uniform float4 _Color;
         uniform float4x4 _WorldToMirror; // set by a script
  
         struct vertexInput {
            float4 vertex : POSITION;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 posInMirror : TEXCOORD0;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            output.posInMirror = mul(_WorldToMirror,
               mul(unity_ObjectToWorld, input.vertex));
            output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            if (input.posInMirror.y > 0.0) 
               // reflection comes out of mirror?
            {
               discard; // don't rasterize it
            }
            return float4(_Color.rgb, 0.0); // set alpha to 0.0 
         }
 
         ENDCG
      }
   }
}