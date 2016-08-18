Shader "Cg_Tutorial/MirrorShader"
{
   Properties {
      _Color ("Mirrors's Color", Color) = (1, 1, 1, 1) 
   } 
   SubShader {
      Tags { "Queue" = "Transparent+10" } 
         // draw after all other geometry has been drawn 
         // because we mess with the depth buffer
      
      // 1st pass: mark mirror with alpha = 0
      Pass { 
         CGPROGRAM 
 
         #pragma vertex vert 
         #pragma fragment frag
 
         float4 vert(float4 vertexPos : POSITION) : SV_POSITION 
         {
            return mul(UNITY_MATRIX_MVP, vertexPos);
         }
 
         float4 frag(void) : COLOR 
         {
            return float4(1.0, 0.0, 0.0, 0.0); 
               // this color should never be visible, 
               // only alpha is important
         }
         ENDCG  
      }

      // 2nd pass: set depth to far plane such that 
      // we can use the normal depth test for the reflected geometry
      Pass { 
         ZTest Always
         Blend OneMinusDstAlpha DstAlpha

         CGPROGRAM 
 
         #pragma vertex vert 
         #pragma fragment frag
 
         uniform float4 _Color; 
            // user-specified background color in the mirror

         float4 vert(float4 vertexPos : POSITION) : SV_POSITION 
         {
            float4 pos = mul(UNITY_MATRIX_MVP, vertexPos);
            pos.z = pos.w;
               // the perspective division will divide pos.z 
               // by pos.w; thus, the depth is 1.0, 
               // which represents the far clipping plane
            return pos;
         }
 
         float4 frag(void) : COLOR 
         {
            return float4(_Color.rgb, 0.0); 
               // set alpha to 0.0 and 
               // the color to the user-specified background color
         }
         ENDCG  
      }
   }
}