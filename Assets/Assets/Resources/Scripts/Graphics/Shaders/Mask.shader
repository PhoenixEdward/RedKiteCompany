﻿Shader "Unlit/Mask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_MainTex2("Texture2", 2D) = "white" {}
		_MainTex3("Texture3", 2D) = "white" {}
    }
    SubShader
    {
        Tags 
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}
        LOD 100

        Pass
        {
			Cull Off
			Lighting Off
			ZWrite Off

			Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };
			sampler2D _MainTex3;
			sampler2D _MainTex2;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
				float2 texelSize = 1 / _ScreenParams.xy;
				fixed4 wallCol = tex2D(_MainTex3, i.vertex * texelSize);
				float4 spriteCol = tex2D(_MainTex2, i.vertex * texelSize);
				
				if (wallCol.a != 0 & wallCol.b > 0.74f)
				{
					return float4(0,0,0,0);
				}
				else
				{
					return col;
				}
				
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
            }
            ENDCG
        }
    }
}
