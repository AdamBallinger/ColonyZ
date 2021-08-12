using ColonyZ.Models.Map;
using ColonyZ.Models.Sprites;
using ColonyZ.Utils;
using UnityEngine;

namespace ColonyZ.Controllers.Render
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class WorldRenderer : MonoBehaviour
    {
        private MeshFilter meshFilter;

        private MeshRenderer meshRenderer;

        [SerializeField] private Texture2D tileTypesTexture;

        private Texture2D mapTexture;
        
        private int Width { get; set; }
        private int Height { get; set; }

        private Color[] colors;

        private void Start()
        {
            SliceTileTypesTexture();
        }

        /// <summary>
        ///     Generates the mesh for the world.
        /// </summary>
        public void GenerateWorldMesh(int _width, int _height)
        {
            Width = _width;
            Height = _height;
            
            if (meshRenderer == null)
            {
                meshRenderer = GetComponent<MeshRenderer>();
                meshFilter = GetComponent<MeshFilter>();
            }

            // offset centre by -0.5f since sprites pivot from centre.
            var meshPivot = new Vector2(Width / 2.0f - 0.5f, Height / 2.0f - 0.5f);

            meshFilter.mesh = MeshUtils.CreateQuad("World Mesh", Width, Height, meshPivot);
            meshRenderer.material.mainTexture = GenerateMapTexture();
            meshRenderer.material.SetInt("WorldWidth", Width);
            meshRenderer.material.SetInt("WorldHeight", Height);
        }

        private Texture2D GenerateMapTexture()
        {
            mapTexture = new Texture2D(Width, Height, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Point
            };

            colors = new Color[mapTexture.width * mapTexture.height];
            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                var index = x * Width + y;
                var tileIndex = World.Instance.GetTileAt(x, y).TileDefinition.TextureIndex / 255.0f;
                colors[index] = new Color(tileIndex, 0, 0);
            }

            mapTexture.SetPixels(colors);
            mapTexture.Apply();

            return mapTexture;
        }

        /// <summary>
        ///     Cuts the tile types texture up into individual sprites.
        /// </summary>
        private void SliceTileTypesTexture()
        {
            for (var y = 0; y < tileTypesTexture.height / 32; y++)
            for (var x = 0; x < tileTypesTexture.width / 32; x++)
            {
                var sprite = Sprite.Create(tileTypesTexture, new Rect(x, y, 32, 32), new Vector2(x + 32, y + 32));
                SpriteCache.AddSprite("Tiles", sprite);
            }
        }
    }
}