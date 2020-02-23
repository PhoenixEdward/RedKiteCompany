#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced '_WorldSpaceCameraPos.w' with '1.0'

Shader "Unlit/Mask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_MainTex2("Texture2", 2D) = "white" {}
		_MainTex3("Texture3", 2D) = "white" {}
		_Covered("Covered", int) = 0
		_FogColor("FogColor", Color) = (0,0,0,0)
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
			ZWrite On

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
				float4 color : COLOR;

            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float4 color : COLOR;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				float4 worldPos : TEXCOORD1;
            };
			sampler2D _MainTex3;
			sampler2D _MainTex2;
            sampler2D _MainTex;
			float4 _FogColor;
			float4 _MainTex_ST;
			int _Covered;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
                UNITY_TRANSFER_FOG(o,o.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
				float2 texelSize = 1 / _ScreenParams.xy;
				fixed4 wallCol = tex2D(_MainTex3, i.vertex * texelSize);
				float4 fogCol = tex2D(_MainTex2, i.vertex * texelSize);
				float dist = (distance(i.worldPos, _WorldSpaceCameraPos) - 18.5) / 16;

				if (wallCol.a != 0 & abs(wallCol.b - dist) < .02f)
				{
					return float4(0,0,0,0);
				}
				else
				{
					return col;
				}
				/*
				else if (fogCol.a != 0)
				{
					return col * (_FogColor * float4(.6f, .6f, .6f, 1));
				}
				*/
				
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
            }
            ENDCG
        }
    }
}
