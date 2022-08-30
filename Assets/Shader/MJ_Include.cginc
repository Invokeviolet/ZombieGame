#ifndef __MIJI_LIGHTING_INCLUDED__
#define  __MIJI_LIGHTING_INCLUDED__

#ifndef _RAMP_HIGHLIGHT_SHADOW_COLORS
#define _RAMP_HIGHLIGHT_SHADOW_COLORS
//Lighting Ramp
sampler2D _Ramp;
	
//Highlight/Shadow Colors
fixed4 _Color;
fixed4 _SColor;
#endif

#ifndef _TREMBLEVECTOR_
#define _TREMBLEVECTOR_

float4 _TrembleVec;

#endif // _TREMBLEVECTOR_
 
struct CustomSurfaceOutput
{
	fixed3 Albedo;  // diffuse color
	fixed3 Normal;  // tangent space normal, if written
	fixed3 Emission;
	half Specular;  // specular power in 0..1 range
	fixed Gloss;    // specular intensity
	fixed Alpha;    // alpha for transparencies
	fixed4 SpecColor;
};

struct CustomSurfaceMaskOutput
{
	fixed3 Albedo;  // diffuse color
	fixed3 Normal;  // tangent space normal, if written
	fixed3 Emission;
	half Specular;  // specular power in 0..1 range
	fixed Gloss;    // specular intensity
	fixed Alpha;    // alpha for transparencies
	fixed4 SpecColor;
	fixed Mask;
	fixed MaskBrightness;
};


// Tremble Vertex function
void vert (inout appdata_full v) 
{
	// Do whatever you want with the "vertex" property of v here
	v.vertex += _TrembleVec;
}

inline half4 LightingMIJIBlinnPhong(CustomSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
{
	fixed diff = max(0, dot(s.Normal, lightDir)*0.5 + 0.5);

	// Ramp shading
	fixed3 ramp = tex2D(_Ramp, fixed2(diff, diff));
	ramp = lerp(_SColor, _Color, ramp);

	// specular
	half3 h = normalize(lightDir + viewDir);
	float nh = max (0, dot (s.Normal, h));
	float spec = pow (nh, s.Specular * 128.0) * s.Gloss;

	fixed4 c;
	c.rgb = (s.Albedo * _LightColor0.rgb * ramp + _LightColor0.rgb * s.SpecColor.rgb * spec) * (atten*2);
	c.a = s.Alpha + _LightColor0.a * s.SpecColor.a * spec * atten;
	return c;
}

inline fixed4 LightingMIJIBlinnPhongMask(CustomSurfaceMaskOutput s, fixed3 lightDir, half3 viewDir, fixed atten)
{
	fixed diff = max(0, dot(s.Normal, lightDir)*0.5 + 0.5);

	// ramp shading
	fixed3 ramp = tex2D(_Ramp, fixed2(diff, diff));
	ramp = lerp(_SColor, _Color, ramp);

	// specular
	half3 h = normalize(lightDir + viewDir);
	float nh = max(0, dot(s.Normal, h));
	float spec = pow(nh, s.Specular*128.0) * s.Gloss;

	
	fixed4 c;
	c.rgb = lerp(s.Albedo * _LightColor0.rgb * ramp + _LightColor0.rgb * s.SpecColor.rgb * spec, s.Albedo * s.MaskBrightness, s.Mask) * (atten*2);
	c.a = s.Alpha + _LightColor0.a * s.SpecColor.a * spec * atten;
	return c;
}

inline fixed4 LightingMIJIBlinnPhong_PrePass(CustomSurfaceOutput s, half4 light)
{
	fixed spec = light.a;// * s.Gloss;
	half d = Luminance(light.rgb);
	half3 ramp = tex2D(_Ramp, float2(d, d));
	ramp = lerp(_SColor + 1, _Color, ramp);

	fixed4 c;
	c.rgb = (s.Albedo * 2 * light.rgb * ramp + light.rgb * s.SpecColor * spec);
	c.a = s.Alpha + spec * s.SpecColor;
	return c;
}

inline fixed4 LightingMIJIBlinnPhongMask_PrePass(CustomSurfaceMaskOutput s, half4 light)
{
	fixed spec = light.a;// * s.Gloss;
	half d = Luminance(light.rgb);
	half3 ramp = tex2D(_Ramp, float2(d, d));
	ramp = lerp(_SColor + 1, _Color, ramp);

	fixed4 c;
	c.rgb = lerp(s.Albedo * 2 * light.rgb * ramp, s.Albedo * s.MaskBrightness, s.Mask);
	c.rgb += lerp(light.rgb * s.SpecColor * spec, 0, s.Mask);
	c.a = s.Alpha + spec * s.SpecColor;
	return c;
}

#endif //  __MIJI_LIGHTING_INCLUDED__