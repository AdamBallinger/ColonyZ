Shader "ColonyZ/WorldMesh"
{
	Properties
	{
		[HideInInspector] _MainTex ("Texture", 2D) = "white" {}
		[NoScaleOffset] _TilesTex ("Tile Textures", 2D) = "white" {}
		[HideInInspector] _WorldWidth ("World Width", Int) = 0
		[HideInInspector] _WorldHeight ("World Height", Int) = 0
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

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

			sampler2D _MainTex;
			sampler2D _TilesTex;
			
			float4 _TilesTex_TexelSize;
			
			int _WorldWidth;
			int _WorldHeight;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
		        int numTiles = 2; // TODO: Auto detect this with _TilesTex_TexelSize.

			    fixed4 sample = tex2D(_MainTex, i.uv);
			    uint index = sample.r;
			    
			    // uv positions for tile tex.
			    uint xPos = index % numTiles;
			    uint yPos = 0;
			    
			    float2 uv = float2(xPos, yPos) / numTiles;
			    
			    float xOff = frac(i.uv.x * _WorldWidth) / numTiles;
			    float yOff = frac(i.uv.y * _WorldHeight);    
			    
			    uv += float2(xOff, yOff);   
			    
				fixed4 col = tex2D(_TilesTex, uv);
				return col;
			}
			ENDCG
		}
	}
}
