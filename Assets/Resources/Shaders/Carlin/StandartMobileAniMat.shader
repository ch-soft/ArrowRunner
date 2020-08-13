Shader "DevCarlin/StandartMobileAniMat"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_SpeedX("SpeedX", Float) = 0
		_SpeedY("SpeedY", Float) = -10


		[Toggle(_SATURATION_BRIGHTNESS_ON)] _ToggleSaturationBrightness("Saturation and Brightness", Int) = 0
		[ShowIf(_SATURATION_BRIGHTNESS_ON)]_Saturation("Saturation", Range(0.0,5.0)) = 1.0
		[ShowIf(_SATURATION_BRIGHTNESS_ON)]_Brightness("Brightness", Range(0.0,5.0)) = 1.0

	}
		SubShader
		{
	 LOD 100

		 Tags
		 {
			 "Queue" = "Transparent+99"
			 "IgnoreProjector" = "True"
			 "RenderType" = "Transparent"
		 }

		 Cull Off
		 Lighting Off
		 ZTest LEqual
		 ZWrite Off
		 Fog { Mode Off }
		 Blend SrcAlpha OneMinusSrcAlpha


			CGPROGRAM

			#pragma surface surf Lambert noforwardadd
			#pragma shader_feature_local _ _SATURATION_BRIGHTNESS_ON
			#pragma shader_feature_local _ _UNLIT_ON

			#pragma shader_feature_local _ _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON _ALPHAMODULATE_ON
			#pragma shader_feature_local _ _COLOR_ON
			//#pragma shader_feature_local _ _MAINTEX_ON
			//#pragma shader_feature_local _ _RECEIVESHADOWS_ON

			sampler2D _MainTex;
			fixed4 _Color;
			float _SpeedX;
			float _SpeedY;

			struct Input
			{
				float2 uv_MainTex;
			};

		 #if _SATURATION_BRIGHTNESS_ON
					float _Saturation;
					float _Brightness;
		 #endif

		struct appdata
		{
			float4 vertex : POSITION;
			float4 color : COLOR;

		 #if _MAINTEX_ON
			float2 uv : TEXCOORD0;
		 #endif
		};

		struct v2f
		{
			float4 pos : SV_POSITION;
			float4 color : COLOR;

		 #if _MAINTEX_ON
					float2 uv : TEXCOORD0;
		 #endif

		 #if _RECEIVESHADOWS_ON
					LIGHTING_COORDS(1, 2)
		 #endif
		};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.color = v.color;

			#if _COLOR_ON
				o.color *= _Color;
			#endif

			#if _MAINTEX_ON
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			#endif

			#if _RECEIVESHADOWS_ON
				TRANSFER_VERTEX_TO_FRAGMENT(o);
			#endif

				return o;
			}


			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = i.color;

			#if _MAINTEX_ON
				col *= tex2D(_MainTex, i.uv);
			#endif

			#if _SATURATION_BRIGHTNESS_ON
				const float3 kLumCoeff = float3(0.2125, 0.7154, 0.0721);
				float l = dot(col.rgb, kLumCoeff);
				float3 intensity = l;
				float3 intensityRGB = float3(lerp(intensity, col.rgb, _Saturation));
				col = fixed4(intensityRGB * _Brightness, col.a);
			#endif


			#if _RECEIVESHADOWS_ON
				float lightAttenuation = LIGHT_ATTENUATION(i);
				float remappedLightAttenuation = remap(0.0f, 1.0f, 1.0f - _ReceiveShadowsPower, 1.0f, lightAttenuation);
				col *= remappedLightAttenuation;
			#endif

				return col;
			}

				/*void surf(Input IN, inout SurfaceOutput o)
				{
					float2 texCoord = IN.uv_MainTex;
					texCoord.x += _Time * _SpeedX;
					texCoord.y += _Time * _SpeedY;
					fixed4 c = tex2D(_MainTex, texCoord) * _Color;
					o.Albedo = c.rgb;

					o.Alpha = c.a;
				}*/


				void surf(Input IN, inout SurfaceOutput o) {
				//o.Albedo = _Color.rgb * _Color.a;
				float2 texCoord = IN.uv_MainTex;
				texCoord.x += _Time * _SpeedX;
				texCoord.y += _Time * _SpeedY;
				fixed4 c = tex2D(_MainTex, texCoord) * _Color;
				o.Albedo = c.rgb;
				o.Alpha = _Color.a;

				o.Emission = _Color.rgb * c;

			}
			fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
			{
				fixed4 c;
				c.rgb = s.Albedo;
				c.a = s.Alpha;
				return c;
			}
			ENDCG
		}
			// FallBack "Diffuse"
				Fallback "Diffuse"
				CustomEditor "GameStudio00ShaderGUI"

}
