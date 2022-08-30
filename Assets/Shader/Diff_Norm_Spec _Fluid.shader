Shader "NarshaGames/Diff_Norm_Spec_Fluid" 
{
	Properties 
	{
		_MainTex("Base (RGB) Gloss (A) ", 2D) = "white" {}
		_BumpMap("Normal map (RGB)", 2D) = "white" {}
		_SpecMap("SpecMap (R)Level (G)Shininess (B)Fluid", 2D) = "black" {}
		_Shininess("Shininess(G)", Range(0.03, 1)) = 0.078125
		_SpecPower("SpecPower(R)", Range(0.03, 10)) = 1

		//Ramp shading
		_Ramp("Toon Ramp (RGB)", 2D) = "gray" {}
		_Color("Highlight Color", Color) = (0.8,0.8,0.8,1)
		_SColor("Shadow Color", Color) = (0.0,0.0,0.0,1)

		_TrembleVec ("Tremble Vector", Vector) = (0,0,0,0)

		_FluidTex("Fluid Tex & Glow (RGB)", 2D) = "black" {}
		_FluidColor("Fluid Color", Color) = (1,1,1,1)
		_UVPanY("UV Y Speed", Range(-5,5) ) = 0
		_UVPanX("UV X Speed", Range(-5,5) ) = 0

		_FluidBrightness ("Fluid Brightness", Range(0.0,2.0)) = 1.0
	}
		

	SubShader 
	{ 
		Tags { "RenderType"="Opaque" }
		LOD 200
	
		CGPROGRAM
		#pragma target 3.0
		#include "MJ_Include.cginc"
		
		#pragma surface surf MIJIBlinnPhongMask nolightmap nodirlightmap noforwardadd approxview halfasview vertex:vert


		sampler2D _MainTex;
		sampler2D _BumpMap;		
		sampler2D _SpecMap;
		half _Shininess;
		half _SpecPower;

		fixed4 _RimColor;
		fixed _RimPower;

		sampler2D _FluidTex;
		fixed4 _FluidColor;
		fixed _UVPanY;
		fixed _UVPanX;

		fixed _FluidBrightness;

		struct Input 
		{
			half2 uv_MainTex;
			half2 uv_FluidTex;
		};

		void surf (Input IN, inout CustomSurfaceMaskOutput o) 
		{
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 specTex = tex2D(_SpecMap, IN.uv_MainTex); 

			// Fluid
			half2 Add0 = half2(IN.uv_FluidTex.x + _Time.x * _UVPanX, IN.uv_FluidTex.y + _Time.x * _UVPanY);
			fixed4 finalEC = tex2D(_FluidTex,Add0) * _FluidColor;

			o.Albedo = lerp(tex, finalEC, specTex.bbbb);
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));

			o.Specular = _Shininess * (1 - specTex.g);		// shininess
			o.SpecColor = _SpecPower * specTex.r;	// specular color
			o.SpecColor.a = 1;
			//o.Gloss = specTex.g;		// Gloss - specular itensity
			o.Gloss = tex.a;
			o.Alpha = tex.a;
			o.Mask = specTex.b;
			o.MaskBrightness = _FluidBrightness;
		}
		ENDCG
	}

	FallBack "Specular"
}
