Shader "NarshaGames/Shader/MijiEnv/RealTime_Diffuse" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_MainColor("Main Color", Color) = (1,1,1,1)
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 150

  CGPROGRAM
  #pragma surface surf Lambert noforwardadd dualforward 

   sampler2D _MainTex;
   fixed4 _MainColor;

   struct Input {
	float2 uv_MainTex;
    };

    void surf (Input IN, inout SurfaceOutput o) {
	    fixed4 c = tex2D(_MainTex, IN.uv_MainTex) *_MainColor *2;;
	          o.Albedo = c.rgb;
	          o.Alpha = c.a;
    }
ENDCG
}

Fallback "Unlit/Texture color (Supports Lightmap)"
}
