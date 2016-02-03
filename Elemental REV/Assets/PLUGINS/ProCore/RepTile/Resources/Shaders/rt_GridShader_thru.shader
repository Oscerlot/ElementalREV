Shader "RepTile/rt_GridShader_thru"
{
	Properties 
	{
		_Difuse("_Difuse", 2D) = "black" {}
		_Opacity("_Opacity", Range(0,1) ) = 0.5
		_Color("_Color", Color) = (0,0.5,0.9,1)
	}
	
	SubShader 
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		}
		
        Lighting Off
		Cull Off
		ZWrite Off
		ZTest GEqual
		ColorMask RGBA
		Blend SrcAlpha OneMinusSrcAlpha
			
		CGPROGRAM 
		#pragma surface surf CustomUnlit nolightmap  nodirlightmap  noambient  novertexlights  keepalpha
		#pragma target 2.0
		
			sampler2D _Difuse;
			float _Opacity;
			float4 _Color;
	
			half4 LightingCustomUnlit (SurfaceOutput s, half3 lightDir, half atten)
			{
				return s.Alpha;
			}
	
			struct Input
			{
				float2 uv_Difuse;
			};
	
			void surf (Input IN, inout SurfaceOutput o)
			{
				float4 Tex2D0 = tex2D(_Difuse,(IN.uv_Difuse.xyxy).xy);
				float4 Min0 = min(Tex2D0.aaaa,_Opacity.xxxx);
	
				o.Albedo = _Color;
				o.Alpha = Min0;
			}
			
		ENDCG
	}
	
	Fallback "Diffuse"
}