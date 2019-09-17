Shader "Custom/PlanetSurface"
{
	Properties
	{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_EmissionColor("Emission Color", Color) = (0,0,0)
	}
		SubShader
		{
			Tags{ "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows
			#pragma multi_compile_instancing
			#pragma instancing_options procedural:setup
			// Use shader model 3.0 target, to get nicer looking lighting
			//#pragma target 3.0

			sampler2D _MainTex;

			struct Input
			{
				float2 uv_MainTex;
			};
			float4 _EmissionColor;
			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				float4 color = 1.0f;
				o.Emission = _EmissionColor;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
