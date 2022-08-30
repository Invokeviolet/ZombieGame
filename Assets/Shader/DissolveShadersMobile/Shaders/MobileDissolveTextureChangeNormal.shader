//Wrote by Andrew M.

Shader "CBG/Dissolve/Mobile/MobileDissolveTextureChangeNormal" 
{

  Properties 
  {
    _DissolvePower ("Dissolve Power", Range(0.75, -0.2)) = 0
    _MainTex ("Main Texture", 2D) = "white"{}
    _MainTexNormal("Main Texture Bump", 2D) = "bump"{}
    _SecondTex ("Second Texture", 2D) = "white"{}
    _SecondTexNormal("Second Texture Bump", 2D) = "bump"{}
    _DissolveTex ("Dissolve Texture", 2D) = "white"{}
  }

  SubShader 
  {
  
    Tags {"IgnoreProjector"="True" "RenderType"="TransparentCutout"}
    LOD 200 

      
    CGPROGRAM
    #pragma surface surf Lambert alphatest:Zero noforwardadd noambient addshadow
      #pragma target 3.0
    
    sampler2D _MainTex;
    sampler2D _MainTexNormal;
    sampler2D _SecondTex;
    sampler2D _SecondTexNormal;
    sampler2D _DissolveTex;
    fixed _DissolvePower;

   struct Input 
   {
     float2 uv_MainTex;
     float2 uv_MainTexNormal;
     float2 uv_SecondTex;
     float2 uv_SecondTexNormal;
     float2 uv_DissolveTex;
   };
 
   void surf (Input IN, inout SurfaceOutput o)
   {     
     half4 tex = tex2D(_MainTex, IN.uv_MainTex);
     half4 stex = tex2D(_SecondTex, IN.uv_SecondTex);
     half4 texd = tex2D(_DissolveTex, IN.uv_DissolveTex);
     
     o.Albedo = tex.rgb;
     o.Gloss = tex.a;
     o.Alpha = _DissolvePower - texd.r;
     
     o.Normal = UnpackNormal(tex2D(_MainTexNormal, IN.uv_MainTexNormal));
     
     if ((o.Alpha < 0)&&(o.Alpha > -1)){
       o.Alpha = 1;
       o.Albedo = (0,0,0,0) + stex.rgb;
       o.Normal = UnpackNormal(tex2D(_SecondTexNormal, IN.uv_SecondTexNormal));       
     }
    } 
       
    ENDCG
  }
}