Shader "NarshaGames/Shader/MijiEnv/BackgroundFlow_Fog" {
	 Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
		_MainColor("Main Color", Color) = (1,1,1,1)
		
	    _UVAniX("UV X Speed", Float) = 0
		_UVAniY("UV Y Speed", Float) = 0
    }
    SubShader {
        Tags { "RenderType"="Opaque"  }
       
        CGPROGRAM
       #pragma surface surf UnlitSimple noforwardadd nolightmap
		
        sampler2D _MainTex;
		fixed4 _MainColor;
		fixed _UVAniX;
		fixed _UVAniY;
		
      
        struct Input {
            fixed2 uv_MainTex;
        };
 
       
		inline fixed4 LightingUnlitSimple(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			return fixed4(s.Albedo, s.Alpha);
		}

        void surf (Input IN, inout SurfaceOutput o) 
		{  
			fixed2 uvani = _Time.x * fixed2(_UVAniX, _UVAniY);
            half4 diff = tex2D (_MainTex, IN.uv_MainTex+uvani);

			
			o.Albedo = diff * _MainColor;
		}
        ENDCG
    }
    FallBack "Mobile/Background"
}
