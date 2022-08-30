﻿Shader "NarshaGames/Shader/Mask/Unlit/MaskUnlitNofog" {
	 Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
		_MainColor("Main Color", Color) = (1,1,1,1)
		_MaskTex ("Mask Texture", 2D) = "white" {}
		_MaskAlpha("Mask Alpha", Float) = 0.5
    }
    SubShader {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		
		Blend SrcAlpha OneMinusSrcAlpha
		Fog { Mode Off }
		ZWrite off
       
        CGPROGRAM
        #pragma surface surf UnlitSimple  alphatest:Zero noforwardadd noambient nolightmap nofog		
		
        sampler2D _MainTex;
		fixed4 _MainColor;
		sampler2D _MaskTex;
		fixed _MaskAlpha;

		
		inline fixed4 LightingUnlitSimple(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			return fixed4(s.Albedo, s.Alpha);
		}

		struct Input {
            fixed2 uv_MainTex;
			fixed2 uv_MaskTex;
        };
 
       
        void surf (Input IN, inout SurfaceOutput o) 
		{  
            half4 diff = tex2D (_MainTex, IN.uv_MainTex);
			half4 mask = tex2D (_MaskTex, IN.uv_MaskTex);
			
			o.Albedo = diff * _MainColor;
			o.Alpha = diff.a * mask.r * _MaskAlpha;
			if( o.Alpha <= 0)
				o.Alpha = -1;
		}
        ENDCG
    }
    FallBack "Mobile/Particles/Alpha Blended"
}
