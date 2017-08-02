Shader "ColonyZ/TileBlend"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		 _BlendTex("Blend Texture", 2D) = "black" {}
		[MaterialToggle] PixelSnap("Pixel Snap", Float) = 0
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

			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Blend SrcAlpha OneMinusSrcAlpha

			Pass
			{
				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile DUMMY PIXELSNAP_ON

				sampler2D _MainTex;
				sampler2D _BlendTex;

				struct Vertex
				{
					float4 vertex : POSITION;
					float2 uv_MainTex : TEXCOORD0;
					float2 uv2 : TEXCOORD1;
				};

				struct Fragment
				{
					float4 vertex : POSITION;
					float2 uv_MainTex : TEXCOORD0;
					float2 uv2 : TEXCOORD1;
				};

				Fragment vert(Vertex v)
				{
					Fragment o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv_MainTex = v.uv_MainTex;
					o.uv2 = v.uv2;

					return o;
				}

				float4 frag(Fragment IN) : COLOR
				{
					float4 o = float4(0, 0, 0, 0);

					half4 c = tex2D(_MainTex, IN.uv_MainTex);
					half4 b = tex2D(_BlendTex, IN.uv_MainTex);
					o.rgb = c.rgb;
					o.a = c.a;

					if(o.a < 1.0)
					{
						o.rgb = b.rgb;
						o.a = b.a;
					}

					return o;
				}

				ENDCG
			}
		}
}
