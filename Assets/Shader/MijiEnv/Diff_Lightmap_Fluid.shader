// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "NarshaGames/Shader/MijiEnv/Lightmapped_Unlit_Fluid"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}

		_UVPanY("UV Y Speed", Range(-5,5)) = 0
		_UVPanX("UV X Speed", Range(-5,5)) = 0
	}

	CGINCLUDE

	#include "UnityCG.cginc"

	sampler2D _MainTex;
	fixed4 _Color;
	float4 _MainTex_ST;
	float _UVPanY;
	float _UVPanX;

	struct appdata_t
	{
		float4 position : POSITION;
		float2 texcoord0 : TEXCOORD0;
		float2 texcoord1 : TEXCOORD1;
	};

	struct v2f
	{
		float4 position : SV_POSITION;
		float2 texcoord0 : TEXCOORD0;
		float2 texcoord1 : TEXCOORD1;
		UNITY_FOG_COORDS(2)
	};

	v2f vert(appdata_t v)
	{
		float2 add0 = float2(_Time.x * _UVPanX, _Time.x * _UVPanY);
		v2f o;
		o.position = UnityObjectToClipPos(v.position);
		o.texcoord0 = v.texcoord0.xy * _MainTex_ST.xy + add0 + _MainTex_ST.zw;
		o.texcoord1 = v.texcoord1 * unity_LightmapST.xy + unity_LightmapST.zw;
		UNITY_TRANSFER_FOG(o, o.position);
		return o;
	}	

	fixed4 frag_nolightmap(v2f i) : SV_Target
	{
		fixed4 col = tex2D(_MainTex, i.texcoord0) * (_Color * 2.0);
		UNITY_APPLY_FOG(i.fogCoord, col);
		return col;
	}

	fixed4 frag_ldr(v2f i) : SV_Target
	{
		fixed4 lm = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.texcoord1);
		fixed4 col = tex2D(_MainTex, i.texcoord0) * lm * 2.0 * (_Color * 2.0);
		UNITY_APPLY_FOG(i.fogCoord, col);
		return col;
	}

	fixed4 frag_rgbm(v2f i) : SV_Target
	{
		half4 lm = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.texcoord1);
		fixed4 col = tex2D(_MainTex, i.texcoord0) * lm * lm.a * 8 * (_Color * 2.0);
		UNITY_APPLY_FOG(i.fogCoord, col);
		return col;
	}

	ENDCG

	SubShader
	{
		Tags{ "RenderType" = "Opaque" }

		Pass{
			Tags{ "LightMode" = "Vertex" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_nolightmap
			#pragma multi_compile_fog
			ENDCG
		}

		Pass
		{
			Tags{ "LightMode" = "VertexLM" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_ldr
			#pragma multi_compile_fog
			ENDCG
		}

		Pass
		{
			Tags{ "LightMode" = "VertexLMRGBM" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_rgbm
			#pragma multi_compile_fog
			ENDCG
		}
	}
}