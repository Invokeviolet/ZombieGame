Shader "NarshaGames/Shader/FluidFlow/Base"
{
	Properties 
	{
		_MainTex("Base Texture (A) Mask", 2D) = "black" {}
		
		_FluidTex("Fluid Tex & Glow (RGB)", 2D) = "black" {}
		_FluidColor("Fluid Color", Color) = (1,1,1,1)
		_UVPanY("UV Y Speed", Range(-4,4) ) = 0
		_UVPanX("UV X Speed", Range(-4,4) ) = 0

		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		_Color ("Highlight Color", Color) = (0.8,0.8,0.8,1)
		_SColor ("Shadow Color", Color) = (0.0,0.0,0.0,1)
		
	}
	
	SubShader 
	{
		Tags { "Queue"="Geometry" "IgnoreProjector"="False" "RenderType"="Opaque" }	
		
		LOD 100

		CGPROGRAM
#pragma surface surf CustomBlinnPhong  exclude_path:prepass nolightmap noforwardadd halfasview novertexlights

			sampler2D _MainTex;
			
			sampler2D _FluidTex;
			float4 _FluidColor;
			float _UVPanY;
			float _UVPanX;

			sampler2D _Ramp;
			fixed4 _Color;
			fixed4 _SColor;

			inline fixed4 LightingCustomBlinnPhong (SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten)
			{
				half3 h = normalize (lightDir + viewDir);	
				fixed diff = max (0, dot(s.Normal, lightDir)*0.5 + 0.5);

				fixed3 ramp = tex2D(_Ramp, fixed2(diff, diff));
				ramp = lerp(_SColor, _Color, ramp);
	
				fixed4 c;
				c.rgb = (s.Albedo * _LightColor0.rgb * ramp) * (atten * 2);
				c.a = s.Alpha + _LightColor0.a * atten;
				return c;
			}
			

			struct Input {
				float2 uv_MainTex;
				float2 uv_FluidTex;
			};

			void surf (Input IN, inout SurfaceOutput o) 
			{	
				// Base
				float4 diffColor = tex2D(_MainTex,IN.uv_MainTex);
				// Fluid
				float4 UV_Pan0=float4(IN.uv_FluidTex.x + _Time.x * _UVPanX, IN.uv_FluidTex.y, IN.uv_FluidTex.x, IN.uv_FluidTex.y);				
				float4 UV_Pan1=float4(IN.uv_FluidTex.x, IN.uv_FluidTex.y + _Time.x * _UVPanY, IN.uv_FluidTex.x, IN.uv_FluidTex.y);
				
				float4 Add0=UV_Pan0 + UV_Pan1;
				
				float4 finalEC =tex2D(_FluidTex,Add0.xy) * _FluidColor;
				float4 Invert0= 1 - diffColor.aaaa;			

				
				o.Albedo = lerp(diffColor, finalEC, Invert0);
				o.Emission = lerp(float4(0,0,0,0), finalEC,Invert0);
				o.Gloss = diffColor.a;
				o.Alpha = diffColor.a * _FluidColor.a;
			}
		ENDCG
	}
	FallBack "Transparent/VertexLit"
}