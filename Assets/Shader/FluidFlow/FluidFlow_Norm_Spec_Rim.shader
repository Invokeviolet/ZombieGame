Shader "NarshaGames/Shader/FluidFlow/NormalSpecularRim"
{
	Properties 
	{
		_MainTex("Base Texture (A) Mask", 2D) = "black" {}
		_Normal("Normal Map", 2D) = "bump" {}	
		
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_SpecMap("Specular map", 2D) = "black" {}
		
		_FluidTex("Fluid Tex & Glow (RGB)", 2D) = "black" {}
		_FluidColor("Fluid Color", Color) = (1,1,1,1)
		_UVPanY("UV Y Speed", Range(-5,5) ) = 0
		_UVPanX("UV X Speed", Range(-5,5) ) = 0

		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		_Color ("Highlight Color", Color) = (0.8,0.8,0.8,1)
		_SColor ("Shadow Color", Color) = (0.0,0.0,0.0,1)

		_RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower ("Rim Power", Range(0.5,8.0)) = 3.0		

		_TrembleVec ("Tremble Vector", Vector) = (0,0,0,0)
	}
	
	SubShader 
	{
		Tags { "Queue"="Geometry" "IgnoreProjector"="False" "RenderType"="Opaque" }	
		
		LOD 100

		CGPROGRAM
#pragma surface surf CustomBlinnPhong  exclude_path:prepass nolightmap noforwardadd halfasview novertexlights vertex:vert

			sampler2D _MainTex;			
			sampler2D _Normal;
			
			float4 _Specular;
			half _Shininess;
			sampler2D _SpecMap;
			
			sampler2D _FluidTex;
			float4 _FluidColor;
			float _UVPanY;
			float _UVPanX;

			sampler2D _Ramp;
			fixed4 _Color;
			fixed4 _SColor;

			float4 _RimColor;
			float _RimPower;

			float4 _TrembleVec;

			inline fixed4 LightingCustomBlinnPhong (SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten)
			{
				half3 h = normalize (lightDir + viewDir);	
				fixed diff = max (0, dot(s.Normal, lightDir)*0.5 + 0.5);

				fixed3 ramp = tex2D(_Ramp, fixed2(diff, diff));
				ramp = lerp(_SColor, _Color, ramp);
	
				float nh = max (0, dot (s.Normal, h));
				float spec = pow (nh, s.Specular*128.0) * s.Gloss;
	
				fixed4 c;
				c.rgb = (s.Albedo * _LightColor0.rgb * ramp + _LightColor0.rgb * spec) * (atten * 2);
				c.a = s.Alpha + _LightColor0.a * spec * atten;
				return c;
			}
			

			struct Input {
				float2 uv_MainTex;
				float2 uv_FluidTex;
				float2 uv_Normal;
				float2 uv_SpecMap;
				float3 viewDir;
			};

			// Vertex modifier function
			void vert (inout appdata_full v) {
				// Do whatever you want with the "vertex" property of v here
				v.vertex += _TrembleVec;
			}

			void surf (Input IN, inout SurfaceOutput o) 
			{	
				// specular map
				fixed4 specTex = tex2D(_SpecMap, IN.uv_SpecMap);
				// Base
				float4 diffColor = tex2D(_MainTex,IN.uv_MainTex);
				// Fluid
				float4 UV_Pan0=float4(IN.uv_FluidTex.x + _Time.x * _UVPanX, IN.uv_FluidTex.y, IN.uv_FluidTex.x, IN.uv_FluidTex.y);				
				float4 UV_Pan1=float4(IN.uv_FluidTex.x, IN.uv_FluidTex.y + _Time.x * _UVPanY, IN.uv_FluidTex.x, IN.uv_FluidTex.y);
				
				float4 Add0=UV_Pan0 + UV_Pan1;
				
				float4 finalEC =tex2D(_FluidTex,Add0.xy) * _FluidColor;
				float4 Invert0= float4(1.0, 1.0, 1.0, 1.0) - diffColor.aaaa;			

				
				o.Albedo = lerp(diffColor, finalEC, Invert0);
				o.Normal = UnpackNormal(tex2D(_Normal,IN.uv_Normal));
				
				half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
				o.Emission = lerp(float4(0,0,0,0), finalEC,Invert0) + _RimColor.rgb * pow (rim, _RimPower);
				
				//o.Specular = _Shininess;
				//o.Gloss = diffColor.a;
				o.Specular = _Shininess * specTex.g;
				o.Gloss = specTex.r;
				o.Alpha = diffColor.a * _FluidColor.a;
			}
		ENDCG
	}
	FallBack "Transparent/VertexLit"
}