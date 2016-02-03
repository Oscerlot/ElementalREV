Shader "RepTile/rt_OpaqueFresnel"
{
	Properties 
	{
		_Color("_Color", Color) = (1,1,1,1)
		_Color2("_Color2", Color) = (1,1,1,1)
		_RimPower("RimPower", Range(0,5)) = 3.0
	}
	
	SubShader 
	{
		Tags
		{
			"Queue"="Geometry"
			"IgnoreProjector"="True"
			"RenderType"="Opaque"
		}
		
        Lighting Off
		Cull Back
		ZWrite On
		ZTest LEqual
		ColorMask RGB
		
		CGPROGRAM
		#pragma surface surf CustomUnlit  nolightmap  nodirlightmap  noambient  novertexlights
		#pragma target 2.0

			float4 _Color;
			float4 _Color2;
			float _RimPower;
			
			half4 LightingCustomUnlit_PrePass (SurfaceOutput s, half4 light)
			{
				return half4(s.Albedo,s.Alpha);
			}
	
			half4 LightingCustomUnlit (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
			{
				return half4(s.Albedo,s.Alpha);
			}
			
			struct Input
			{
				float3 viewDir;
				float3 worldNormal;
			};

			void surf (Input IN, inout SurfaceOutput o)
			{
				half fresnel =  max(0.001f, dot(IN.viewDir, IN.worldNormal) );
				half rim = 1.0 -  saturate(fresnel);
				
				o.Albedo = lerp(_Color,_Color2, clamp( pow(rim,_RimPower), 0.0, 1.0) );
				o.Emission = o.Albedo;
			}
		ENDCG
	}
	
	Fallback "Diffuse"
}