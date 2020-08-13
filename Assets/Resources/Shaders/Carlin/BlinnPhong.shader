Shader "DevCarlin/BlinnPhong"
{
    Properties
    {
        [Toggle(_COLOR_ON)] _ToggleColor("Color", Int) = 0
		[ShowIf(_COLOR_ON)]_Color("", Color) = (1,1,1,1)

		[Toggle(_MAINTEX_ON)] _ToggleMainTex("Texture", Int) = 0
		[ShowIf(_MAINTEX_ON)]_MainTex("", 2D) = "white" {}

		[Toggle(_EMISSION_ON)] _ToggleEmission("Emission", Int) = 0
		[ShowIf(_EMISSION_ON)][HDR]_EmissionColor("", Color) = (1,1,1)
		[ShowIf(_EMISSION_ON)]_EmissionMap("", 2D) = "white" {}

		[PowerSlider(5.0)] _Shininess("Shininess", Range (0.03, 1) ) = 0.078125

		[ShowIf(_ALPHATEST_ON)]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5

		// Hidden properties
		[HideInInspector] _Mode ("__mode", Float) = 0.0
		[HideInInspector] _BlendOp ("__blendop", Float) = 0.0
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _ZWrite ("__zw", Float) = 1.0
		[HideInInspector] _ZTest ("__zt", Float) = 2.0
		[HideInInspector] _Cull ("__cull", Float) = 2.0
    }

	CGINCLUDE

	ENDCG

    SubShader
    {
		BlendOp [_BlendOp]
		Blend [_SrcBlend] [_DstBlend]
        ZWrite [_ZWrite]
		ZTest [_ZTest]
		Cull [_Cull]


        LOD 150

        CGPROGRAM

		#pragma shader_feature_local _ _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON _ALPHAMODULATE_ON
		#pragma shader_feature_local _ _COLOR_ON
		#pragma shader_feature_local _ _MAINTEX_ON
		#pragma shader_feature_local _ _EMISSION_ON
        
		#pragma surface surf MobileBlinnPhong exclude_path:prepass nolightmap noforwardadd halfasview interpolateview keepalpha


		inline fixed4 LightingMobileBlinnPhong( SurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten )
		{
			fixed diff = max (0, dot (s.Normal, lightDir));
			fixed nh = max (0, dot (s.Normal, halfDir));
			fixed spec = pow (nh, s.Specular*128) * s.Gloss;

			fixed4 c;
			c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;


		#if _ALPHABLEND_ON || _ALPHAPREMULTIPLY_ON || _ALPHAMODULATE_ON
			c.a = s.Alpha;
		#else
			UNITY_OPAQUE_ALPHA( c.a );
		#endif
			return c;
		}


        #if _COLOR_ON
			float4 _Color;
		#endif

		#if _MAINTEX_ON
			sampler2D _MainTex;
		#endif

		#if _EMISSION_ON
			float4 _EmissionColor;
			sampler2D _EmissionMap;
		#endif

		half _Shininess;

        struct Input
        {
            float2 uv_MainTex;
        };

       

        void surf (Input IN, inout SurfaceOutput o)
        {
			fixed4 c = fixed4(1,1,1,1);

			#if _COLOR_ON
				c *= _Color;
			#endif

			#if _MAINTEX_ON
				c *= tex2D( _MainTex, IN.uv_MainTex );
			#endif

			#if _EMISSION_ON
				o.Emission = tex2D( _EmissionMap, IN.uv_MainTex ) * _EmissionColor;
			#endif

			o.Gloss = c.a;
			o.Specular = _Shininess;

            o.Albedo = c.rgb;
            o.Alpha = c.a;

        }
        ENDCG
    }
    FallBack "Mobile/VertexLit"
	CustomEditor "DisquireShaderGUI"
}
