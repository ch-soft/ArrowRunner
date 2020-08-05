﻿Shader "DevCarlin/Water" {
	Properties{
		[Toggle(_COLOR_ON)] _ToggleColor("Color", Int) = 0
		[ShowIf(_COLOR_ON)]_Color("", Color) = (1,1,1,1)

		[Toggle(_MAINTEX_ON)] _ToggleMainTex("Texture", Int) = 0
		[ShowIf(_MAINTEX_ON)]_MainTex("", 2D) = "white" {}

		[Toggle(_SATURATION_BRIGHTNESS_ON)] _ToggleSaturationBrightness("Saturation and Brightness", Int) = 0
		[ShowIf(_SATURATION_BRIGHTNESS_ON)]_Saturation( "Saturation", Range( 0.0,5.0 ) ) = 1.0
		[ShowIf(_SATURATION_BRIGHTNESS_ON)]_Brightness( "Brightness", Range( 0.0,5.0 ) ) = 1.0

		[Toggle(_RECEIVESHADOWS_ON)]_ToggleReceiveShadows("Receive Shadows", Int) = 0
		[ShowIf(_RECEIVESHADOWS_ON)]_ReceiveShadowsPower("", Range(0.0,1.0) ) = 0.5


		_BaseColor("Base Color", Color) = (1,1,1,1)
		_RippleSpeed("Ripple Speed", Float ) = 1.0
		_RippleSize("Ripple Size", Float ) = 5.0
		_RippleTest("Ripple Test", Float ) = 1.0
		_RippleSlimness("Ripple Slimness", Float ) = 5.0
		[HDR]_RippleColor("Ripple Color", Color) = (1,1,1,1)

		// Hidden properties
		[HideInInspector] _Mode ("__mode", Float) = 0.0
		[HideInInspector] _BlendOp ("__blendop", Float) = 0.0
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _ZWrite ("__zw", Float) = 1.0
		[HideInInspector] _ZTest ("__zt", Float) = 2.0
		[HideInInspector] _Cull ("__cull", Float) = 2.0
	}
	SubShader
	{
		BlendOp [_BlendOp]
		Blend [_SrcBlend] [_DstBlend]
        ZWrite [_ZWrite]
		ZTest [_ZTest]
		Cull [_Cull]
		

		Pass
		{
			CGPROGRAM

			#pragma shader_feature_local _ _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON _ALPHAMODULATE_ON
			#pragma shader_feature_local _ _COLOR_ON
			#pragma shader_feature_local _ _MAINTEX_ON
			#pragma shader_feature_local _ _RECEIVESHADOWS_ON
			#pragma shader_feature_local _ _SATURATION_BRIGHTNESS_ON

		#if _RECEIVESHADOWS_ON
			#pragma multi_compile_fwdbase
		#endif

			#pragma vertex vert
			#pragma fragment frag
			

			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#include "Lighting.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;

				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 color : COLOR;

				float2 uv : TEXCOORD0;

			#if _RECEIVESHADOWS_ON
				LIGHTING_COORDS(1,2)
			#endif
			};

			
		#if _COLOR_ON
			float4 _Color;
		#endif

		#if _MAINTEX_ON
			sampler2D _MainTex;
		#endif

		#if _RECEIVESHADOWS_ON
			float _ReceiveShadowsPower;
		#endif

		#if _SATURATION_BRIGHTNESS_ON
			float _Saturation;
			float _Brightness;
		#endif

			float4 _MainTex_ST;

			float4 _BaseColor;
			float _RippleSpeed;
			float _RippleSize;
			float _RippleTest;
			float _RippleSlimness;
			float4 _RippleColor;

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.color = v.color;

			#if _COLOR_ON
				o.color *= _Color;
			#endif

			//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			o.uv = v.uv;

			#if _RECEIVESHADOWS_ON
				TRANSFER_VERTEX_TO_FRAGMENT(o);
			#endif
			
				return o;
			}


			float remap( 
				float inMin, float inMax,
				float outMin, float outMax,
				float value )
			{
				return outMin + (value - inMin) * (outMax - outMin) / (inMax - inMin);
			}

			float2 random2( float2 p ) 
			{
				return frac( sin ( float2( dot( p, float2(127.1,311.7) ), dot( p, float2(269.5,183.3) ) ) )*43758.5453);
			}

			float voronoi( float2 st )
			{
				st *= _RippleSize;

				// Tile the space
				float2 i_st = floor(st);
				float2 f_st = frac(st);

				float result = 1.0;  // minimun distance

				for( int y= -1; y <= 1; y++ ) 
				{
					for( int x= -1; x <= 1; x++ ) 
					{
						// Neighbor place in the grid
						float2 neighbor = float2( float(x), float(y) );

						// Random position from current + neighbor place in the grid
						float2 p = random2( i_st + neighbor );

						// Animate the point
						p = 0.5 + 0.5*sin( _Time[1] * _RippleSpeed + 6.2831 * p);

						// Vector between the pixel and the point
						float2 diff = neighbor + p - f_st;

						// Distance to the point
						float dist = length(diff);

						// Keep the closer distance
						result = min( result, dist);
					}
				}

				return result;
			}


			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = i.color;

			#if _MAINTEX_ON
				col *= tex2D( _MainTex, i.uv );
			#endif

			#if _SATURATION_BRIGHTNESS_ON
				const float3 kLumCoeff = float3( 0.2125, 0.7154, 0.0721 );
				float l = dot( col.rgb, kLumCoeff );
				float3 intensity = l;
				float3 intensityRGB =  float3( lerp( intensity, col.rgb, _Saturation ) );
				col = fixed4( intensityRGB * _Brightness, col.a );
			#endif


			#if _RECEIVESHADOWS_ON
				float lightAttenuation = LIGHT_ATTENUATION(i);
				float remappedLightAttenuation = remap( 0.0f, 1.0f, 1.0f - _ReceiveShadowsPower, 1.0f, lightAttenuation );
				col *= remappedLightAttenuation;
			#endif

				float v = voronoi( i.uv );

				v = pow( v, _RippleSlimness );

				col.rgb = lerp( _BaseColor.rgb, _RippleColor.rgb,  1.0 - v );
				col.a = _BaseColor.a;

				return col;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "DisquireShaderGUI"
}
