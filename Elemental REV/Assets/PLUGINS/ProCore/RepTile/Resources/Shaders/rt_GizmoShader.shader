Shader "RepTile/rt_GizmoShader"
{
	Properties 
	{
		_Difuse("_Difuse", 2D) = "black" {}
		_Opacity("_Opacity", Range(0,1) ) = 0.5
		_Color("_Color", Color) = (1,1,1,1)
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
		ZTest LEqual
		ColorMask RGBA
		Blend SrcAlpha OneMinusSrcAlpha
		Offset -1,-1

		CGPROGRAM
		#pragma surface surf CustomUnlit nolightmap  nodirlightmap  noambient  novertexlights  keepalpha
		#pragma target 2.0
		
			sampler2D _Difuse;
			float _Opacity;
			float4 _Color;
			
			half4 LightingCustomUnlit (SurfaceOutput s, half3 lightDir, half atten)
			{
				return half4(s.Albedo,s.Alpha);
			}
	
			struct Input
			{
				float2 uv_Difuse;
			};
	
			void surf (Input IN, inout SurfaceOutput o)
			{
				float4 Tex2D0 = tex2D(_Difuse,(IN.uv_Difuse.xyxy).xy);
				float4 Min0 = min(Tex2D0.aaaa,_Opacity.xxxx);
	
				o.Albedo = _Color.rgb;
				o.Alpha = Min0;
			}
			
		ENDCG
	}
	
	Fallback "Diffuse"
}