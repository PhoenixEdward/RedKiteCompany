// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/GlowMask"
{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_MainTex2("Base(RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_Covered("Covered", int) = 0
	}
		SubShader{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}			Cull Off
			Cull Off
			Lighting Off
			ZWrite Off

			Blend SrcAlpha OneMinusSrcAlpha

			Pass {

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				sampler2D _MainTex;

				struct v2f {
					float4 pos : SV_POSITION;
					half2 uv : TEXCOORD0;
				};

				v2f vert(appdata_base v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = v.texcoord;
					return o;
				}

				fixed4 _Color;
				float4 _MainTex_TexelSize;
				int _Covered;
				sampler2D _MainTex2;

				fixed4 frag(v2f i) : COLOR
				{
				float2 texelSize = 1 / _ScreenParams.xy;
				fixed4 wall = tex2D(_MainTex2, i.pos * texelSize);
					half4 c = tex2D(_MainTex, i.uv);
					c.rgb *= c.a;
					half4 outlineC = _Color;
					//outlineC.a *= ceil(c.a);
					outlineC.rgb *= outlineC.a;

					fixed alpha_up = tex2D(_MainTex, i.uv + fixed2(0, _MainTex_TexelSize.y)).a;
					fixed alpha_down = tex2D(_MainTex, i.uv - fixed2(0, _MainTex_TexelSize.y)).a;
					fixed alpha_right = tex2D(_MainTex, i.uv + fixed2(_MainTex_TexelSize.x, 0)).a;
					fixed alpha_left = tex2D(_MainTex, i.uv - fixed2(_MainTex_TexelSize.x, 0)).a;

					fixed alpha_up2 = tex2D(_MainTex, i.uv + fixed2(0, _MainTex_TexelSize.y * 2)).a;
					fixed alpha_down2 = tex2D(_MainTex, i.uv - fixed2(0, _MainTex_TexelSize.y * 2)).a;
					fixed alpha_right2 = tex2D(_MainTex, i.uv + fixed2(_MainTex_TexelSize.x * 2, 0)).a;
					fixed alpha_left2 = tex2D(_MainTex, i.uv - fixed2(_MainTex_TexelSize.x * 2, 0)).a;

					if (_Covered == 1)
					{
						if (c.a != 1 & wall.a == 1)
						{
							return lerp(c, outlineC, c.a == 0 && (alpha_up + alpha_down + alpha_right > 0) || (alpha_left + alpha_up2 + alpha_down2 + alpha_left2 + alpha_right2 > 0));
						}
						else if (c.a == 1 & wall.a == 1)
						{
							return float4(outlineC.r, outlineC.g, outlineC.b, 0.75f);
						}
						else
						{
							return float4(0, 0, 0, 0);
						}
					}
					else
					{
						return float4(0, 0, 0, 0);
					}
				}

				ENDCG
			}
		}
			FallBack "Diffuse"
}
