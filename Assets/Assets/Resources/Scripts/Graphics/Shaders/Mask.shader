Shader "Unlit/Mask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_MainTex2("Texture2", 2D) = "white" {}
		_MainTex3("Texture3", 2D) = "white" {}
		_Covered("IsCovered", int) = 0
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
				float4 color : COLOR;

            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float4 color : COLOR;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };
			sampler2D _MainTex3;
			sampler2D _MainTex2;
            sampler2D _MainTex;
            float4 _MainTex_ST;
			int _Covered;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
				float2 texelSize = 1 / _ScreenParams.xy;
				fixed4 wallCol = tex2D(_MainTex3, i.vertex * texelSize);
				float4 fogCol = tex2D(_MainTex2, i.vertex * texelSize);
				
				if (wallCol.a != 0 & wallCol.b > 0.74f)
				{
					_Covered = 1;
					return float4(0,0,0,0);
				}
				else if (fogCol.a != 0)
				{
					return col * float4(0.6f, 0.345f, 0.196f, 1);
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
