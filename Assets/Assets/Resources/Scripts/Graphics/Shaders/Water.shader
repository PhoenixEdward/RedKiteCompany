Shader "Custom/Water"
{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_RotationSpeed("Rotation Speed", Float) = 1
	}
		SubShader
		{

			Cull Off
			Lighting Off
			ZTest Off
			ZWrite Off

			Blend SrcAlpha OneMinusSrcAlpha

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

			Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			sampler2D _MainTex;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float _RotationSpeed;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				float sinX = sin(_RotationSpeed * _Time.x);
				float cosX = cos(_RotationSpeed * _Time.x);
				float sinY = sin(_RotationSpeed * _Time.x);
				float2x2 rotationMatrix = float2x2(cosX, -sinX, sinY, cosX);
				o.uv.xy = mul(v.uv.xy, rotationMatrix);
				return o;
			};

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

			float norm = ((i.uv.x * i.uv.y) * 0.5) + .5f;

				return float4(col.r, col.g, col.b, norm);
			};
			ENDCG
		}
	}
}