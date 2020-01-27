Shader "Unlit/Fog"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_MainTex2 ("Texture2", 2D) = "white" {}
		_MainTex3("Texture3", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_DissipateAlpha("DissipateRate", float) = 0
		_TexDimensions("TextureDimensions", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" 
				
			"Queue"="Transparent"
			}
        LOD 100

			Cull Off
			Lighting Off
			ZWrite Off

			Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
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

            sampler2D _MainTex;
			sampler2D _MainTex2;
			sampler2D _MainTex3;
			float4 _Color;
            float4 _MainTex_ST;
			float _DissipateAlpha;
			float _TexDimensions;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 primCol = tex2D(_MainTex, i.uv);

				float2 texelSize = _TexDimensions / 1;

				if(primCol.a != 0)
				{
					float offset = (_Time.y % 10)/10;

					float2 newUV = (i.uv * texelSize) + float2(offset, offset);

					if (newUV.x > 1)
					{
						newUV.x = newUV.x - 1;
					}
					if (newUV.y > 1)
					{
						newUV.y = newUV.y - 1;
					}


					// sample the texture
					fixed4 col = tex2D(_MainTex2, newUV);

					float alphaReduction = _DissipateAlpha <= 0.65f ? _DissipateAlpha : 0.65f;
					// apply fog
					return float4(col.r, col.g, col.b, primCol.a * (.65f - alphaReduction)) * _Color;
				}
				else
				{
					return float4(0, 0, 0, 0);
				}

            }
            ENDCG
        }
    }
}
