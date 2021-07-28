using ColonyZ.Models.Map;
using ColonyZ.Models.Sprites;
using UnityEngine;
using UnityEngine.Rendering;

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

            var mesh = new Mesh
            {
                name = "World Mesh",
                indexFormat = IndexFormat.UInt16
            };

            var verts = new Vector3[4];
            verts[0] = new Vector3(-0.5f, -0.5f);
            verts[1] = new Vector3(-0.5f, Height - 0.5f);
            verts[2] = new Vector3(Width - 0.5f, Height - 0.5f);
            verts[3] = new Vector3(Width - 0.5f, -0.5f);

            var tris = new int[6];
            tris[0] = 0;
            tris[1] = 1;
            tris[2] = 3;
            tris[3] = 1;
            tris[4] = 2;
            tris[5] = 3;

            var uv = new Vector2[4];
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(0, 1);
            uv[2] = new Vector2(1, 1);
            uv[3] = new Vector2(1, 0);

            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.uv = uv;

            meshFilter.mesh = mesh;
            meshRenderer.material.mainTexture = GenerateMapTexture();
            meshRenderer.material.SetInt("_WorldWidth", Width);
            meshRenderer.material.SetInt("_WorldHeight", Height);
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
                var tileIndex = World.Instance.GetTileAt(x, y).TileDefinition.TextureIndex;
                colors[index] = new Color(tileIndex, tileIndex, tileIndex);
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