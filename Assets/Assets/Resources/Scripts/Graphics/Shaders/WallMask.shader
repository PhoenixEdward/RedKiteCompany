Shader "Unlit/WallMask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_MainTex2("Texture2", 2D) = "White" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" 
				"IgnoreProjector" = "True"	}
        LOD 100

        Pass
        {
			Cull Off
			Lighting Off
			ZWrite On

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
				float4 worldPos : TEXCOORD1;
			};

            sampler2D _MainTex;
            float4 _MainTex_ST;
			sampler2D _CameraDepthTexture;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
			
				float dist = (distance(i.worldPos, _WorldSpaceCameraPos) - 18.5) / 16;

			fixed4 col = tex2D(_MainTex, i.uv);

			if (col.a != 0)
			{
				return float4(0, i.uv.y, dist, 1);
			}
			else
			{
				return float4(0, 0, 0, 0);
			}
			
                // apply fog
            }
            ENDCG
        }
    }
}
